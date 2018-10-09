using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class ResponseTarsConvert : TarsConvertBase<Response>
    {
        private readonly ITarsConvert<short> shortConvert;
        private readonly ITarsConvert<int> intConvert;
        private readonly ITarsConvert<byte> byteConvert;
        private readonly ITarsConvert<string> stringConvert;
        private readonly IDictionaryInterfaceTarsConvert<string, string> dictConvert;
        private readonly ITarsConvert<IByteBuffer> bufferConvert;
        private readonly ITarsConvertRoot convertRoot;
        private readonly IRpcMetadata rpcMetadata;
        private readonly ITarsHeadHandler headHandler;

        public ResponseTarsConvert(ITarsConvert<short> shortConvert, ITarsConvert<int> intConvert,
            ITarsConvert<byte> byteConvert, ITarsConvert<string> stringConvert,
            IDictionaryInterfaceTarsConvert<string, string> dictConvert, ITarsConvert<IByteBuffer> bufferConvert,
            ITarsConvertRoot convertRoot, IRpcMetadata rpcMetadata, ITarsHeadHandler headHandler)
        {
            this.shortConvert = shortConvert;
            this.intConvert = intConvert;
            this.byteConvert = byteConvert;
            this.stringConvert = stringConvert;
            this.dictConvert = dictConvert;
            this.bufferConvert = bufferConvert;
            this.convertRoot = convertRoot;
            this.rpcMetadata = rpcMetadata;
            this.headHandler = headHandler;
        }

        public override Response Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var resp = new Response();
            headHandler.ReadHead(buffer, options);
            options.Version = resp.Version = shortConvert.Deserialize(buffer, options);
            switch (options.Version)
            {
                case TarsCodecsVersion.V2:
                case TarsCodecsVersion.V3:
                    DeserializeV2OrV3(buffer, options, resp);
                    break;

                default:
                    DeserializeV1(buffer, options, resp);
                    break;
            }
            return resp;
        }

        private void DeserializeV2OrV3(IByteBuffer buffer, TarsConvertOptions options, Response resp)
        {
            while (buffer.IsReadable())
            {
                headHandler.ReadHead(buffer, options);
                switch (options.Tag)
                {
                    case 2:
                        resp.PacketType = byteConvert.Deserialize(buffer, options);
                        break;

                    case 3:
                        resp.MessageType = intConvert.Deserialize(buffer, options);
                        break;

                    case 4:
                        resp.RequestId = intConvert.Deserialize(buffer, options);
                        break;

                    case 5:
                        resp.ServantName = stringConvert.Deserialize(buffer, options);
                        break;

                    case 6:
                        resp.FuncName = stringConvert.Deserialize(buffer, options);
                        var (method, isOneway, outParameters, codec, version, serviceType) = rpcMetadata.FindRpcMethod(resp.ServantName, resp.FuncName);
                        resp.ReturnValueType = method.ReturnParameter;
                        resp.ReturnParameterTypes = outParameters;
                        resp.ReturnParameters = new object[outParameters.Length];
                        break;

                    case 7 when options.Version == TarsCodecsVersion.V2:
                        {
                            var uni = new UniAttributeV2(convertRoot, headHandler);
                            uni.Deserialize(buffer, options);
                            var buf = uni.Temp[string.Empty].Values.First();
                            headHandler.ReadHead(buf, options);
                            resp.ReturnValue = convertRoot.Deserialize(buf, resp.ReturnValueType.ParameterType, options);
                            for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
                            {
                                var pt = resp.ReturnParameterTypes[i];
                                buf = uni.Temp[pt.Name].Values.First();
                                headHandler.ReadHead(buf, options);
                                resp.ReturnParameters[i] = convertRoot.Deserialize(buf, pt.ParameterType, options);
                            }
                        }
                        break;

                    case 7 when options.Version == TarsCodecsVersion.V3:
                        {
                            var uni = new UniAttributeV3(convertRoot, headHandler);
                            uni.Deserialize(buffer, options);
                            if (uni.Temp.ContainsKey(string.Empty))
                            {
                                var buf = uni.Temp[string.Empty];
                                headHandler.ReadHead(buf, options);
                                resp.ReturnValue = convertRoot.Deserialize(buf, resp.ReturnValueType.ParameterType, options);
                            }
                            else if (resp.ReturnValueType.ParameterType.GetTypeInfo().IsTask())
                            {
                                resp.ReturnValue = Task.CompletedTask;
                            }
                            for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
                            {
                                var pt = resp.ReturnParameterTypes[i];
                                var buf = uni.Temp[pt.Name];
                                headHandler.ReadHead(buf, options);
                                resp.ReturnParameters[i] = convertRoot.Deserialize(buf, pt.ParameterType, options);
                            }
                        }
                        break;

                    case 8:
                        resp.Timeout = intConvert.Deserialize(buffer, options);
                        break;

                    case 9:
                        resp.Context = dictConvert.Deserialize(buffer, options);
                        break;

                    case 10:
                        resp.Status = dictConvert.Deserialize(buffer, options);
                        break;
                }
            }
        }

        private void DeserializeV1(IByteBuffer buffer, TarsConvertOptions options, Response resp)
        {
            while (buffer.IsReadable())
            {
                headHandler.ReadHead(buffer, options);
                switch (options.Tag)
                {
                    case 2:
                        resp.PacketType = byteConvert.Deserialize(buffer, options);
                        break;

                    case 3:
                        resp.RequestId = intConvert.Deserialize(buffer, options);
                        break;

                    case 4:
                        resp.MessageType = intConvert.Deserialize(buffer, options);
                        break;

                    case 5:
                        resp.ResultStatusCode = (RpcStatusCode)intConvert.Deserialize(buffer, options);
                        break;

                    case 6:
                        var contentBuffer = bufferConvert.Deserialize(buffer, options);
                        if (resp.ResultStatusCode == RpcStatusCode.ServerSuccess)
                        {
                            var returnType = resp.ReturnValueType.ParameterType;
                            if (returnType != typeof(void))
                            {
                                if (returnType == typeof(Task) || returnType == typeof(ValueType))
                                {
                                    resp.ReturnValue = Task.CompletedTask;
                                }
                                else
                                {
                                    headHandler.ReadHead(contentBuffer, options);
                                    if (returnType.BaseType == typeof(Task))
                                    {
                                        var resultType = returnType.GenericTypeArguments[0];
                                        resp.ReturnValue = Task.FromResult(convertRoot.Deserialize(contentBuffer, resultType, options));
                                    }
                                    else if (returnType.BaseType == typeof(ValueType))
                                    {
                                        var resultType = returnType.GenericTypeArguments[0];
                                        var resultItem = convertRoot.Deserialize(contentBuffer, resultType, options);
                                        resp.ReturnValue = Activator.CreateInstance(returnType, new object[1] { resultItem });
                                    }
                                    else
                                    {
                                        resp.ReturnValue = convertRoot.Deserialize(contentBuffer, returnType, options);
                                    }
                                }
                            }
                            if (resp.ReturnParameterTypes.Length > 0)
                            {
                                if (resp.ReturnParameters == null)
                                {
                                    resp.ReturnParameters = new object[resp.ReturnParameterTypes.Length];
                                }

                                for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
                                {
                                    options.Tag = i + 1;
                                    resp.ReturnParameters[i] = convertRoot.Deserialize(contentBuffer, resp.ReturnParameterTypes[i].ParameterType, options);
                                }
                            }
                        }
                        break;

                    case 7:
                        resp.Status = dictConvert.Deserialize(buffer, options);
                        break;

                    case 8:
                        resp.ResultDesc = stringConvert.Deserialize(buffer, options);
                        break;

                    case 9:
                        resp.Context = dictConvert.Deserialize(buffer, options);
                        break;
                }
            }
        }

        public override void Serialize(Response obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            options.Tag = 1;
            options.Version = obj.Version;
            shortConvert.Serialize(obj.Version, buffer, options);
            options.Tag = 2;
            byteConvert.Serialize(obj.PacketType, buffer, options);
            switch (options.Version)
            {
                case TarsCodecsVersion.V2:
                case TarsCodecsVersion.V3:
                    SerializeV2OrV3(obj, buffer, options);
                    break;

                default:
                    SerializeV1(obj, buffer, options);
                    break;
            }
        }

        private void SerializeV2OrV3(Response obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            options.Tag = 3;
            intConvert.Serialize(obj.MessageType, buffer, options);
            options.Tag = 4;
            intConvert.Serialize(obj.RequestId, buffer, options);
            options.Tag = 5;
            stringConvert.Serialize(obj.ServantName, buffer, options);
            options.Tag = 6;
            stringConvert.Serialize(obj.FuncName, buffer, options);
            if (options.Version == TarsCodecsVersion.V2)
            {
                var uni = new UniAttributeV2(convertRoot, headHandler)
                {
                    Temp = new Dictionary<string, IDictionary<string, IByteBuffer>>()
                };
                var type = obj.ReturnValueType.ParameterType;
                if (obj.ReturnValue != null && type != typeof(void) && type != typeof(Task))
                {
                    uni.Put(string.Empty, obj.ReturnValue, obj.ReturnValueType.ParameterType, options);
                }

                if (obj.ReturnParameterTypes != null)
                {
                    for (int i = 0; i < obj.ReturnParameterTypes.Length; i++)
                    {
                        var pt = obj.ReturnParameterTypes[i];
                        uni.Put(pt.Name, obj.ReturnParameters[i], pt.ParameterType, options);
                    }
                }
                options.Tag = 7;
                uni.Serialize(buffer, options);
            }
            else
            {
                var uni = new UniAttributeV3(convertRoot, headHandler)
                {
                    Temp = new Dictionary<string, IByteBuffer>()
                };
                var type = obj.ReturnValueType.ParameterType;
                if (obj.ReturnValue != null && type != typeof(void) && type != typeof(Task))
                {
                    uni.Put(string.Empty, obj.ReturnValue, obj.ReturnValueType.ParameterType, options);
                }
                if (obj.ReturnParameterTypes != null)
                {
                    for (int i = 0; i < obj.ReturnParameterTypes.Length; i++)
                    {
                        var pt = obj.ReturnParameterTypes[i];
                        uni.Put(pt.Name, obj.ReturnParameters[i], pt.ParameterType, options);
                    }
                }
                options.Tag = 7;
                uni.Serialize(buffer, options);
            }
            options.Tag = 8;
            intConvert.Serialize(obj.Timeout, buffer, options);
            options.Tag = 9;
            dictConvert.Serialize(obj.Context, buffer, options);
            options.Tag = 10;
            dictConvert.Serialize(obj.Status, buffer, options);
        }

        private void SerializeV1(Response obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            options.Tag = 3;
            intConvert.Serialize(obj.RequestId, buffer, options);
            options.Tag = 4;
            intConvert.Serialize(obj.MessageType, buffer, options);
            options.Tag = 5;
            intConvert.Serialize((int)obj.ResultStatusCode, buffer, options);
            var contentBuffer = Unpooled.Buffer(128);
            if (obj.ResultStatusCode == RpcStatusCode.ServerSuccess)
            {
                var type = obj.ReturnValueType.ParameterType;
                if (type != typeof(void) && type != typeof(Task))
                {
                    //0的位置是专门给返回值用的
                    options.Tag = 0;
                    if (type.GetTypeInfo().IsTaskWithResult())
                    {
                        convertRoot.Serialize(type.GetProperty("Result").GetReflector().GetValue(obj.ReturnValue), type.GenericTypeArguments[0], contentBuffer, options);
                    }
                    else
                    {
                        convertRoot.Serialize(obj.ReturnValue, type, contentBuffer, options);
                    }
                }
                int outResIndex = 0;
                foreach (var item in obj.ReturnParameterTypes)
                {
                    options.Tag = item.Position + 1;
                    convertRoot.Serialize(obj.ReturnParameters[outResIndex++], item.ParameterType, contentBuffer, options);
                }
            }
            options.Tag = 6;
            bufferConvert.Serialize(contentBuffer, buffer, options);
            if (obj.Status != null)
            {
                options.Tag = 7;
                dictConvert.Serialize(obj.Status, buffer, options);
            }
            if (obj.ResultStatusCode != RpcStatusCode.ServerSuccess)
            {
                options.Tag = 8;
                stringConvert.Serialize(obj.ResultDesc, buffer, options);
            }
            if (obj.Context != null)
            {
                options.Tag = 9;
                dictConvert.Serialize(obj.Context, buffer, options);
            }
        }
    }
}