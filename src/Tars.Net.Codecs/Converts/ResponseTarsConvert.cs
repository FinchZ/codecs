using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using System;
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
        private readonly IDictionaryTarsConvert<string, string> dictConvert;
        private readonly ITarsConvert<IByteBuffer> bufferConvert;
        private readonly ITarsConvertRoot convertRoot;

        public ResponseTarsConvert(ITarsConvert<short> shortConvert, ITarsConvert<int> intConvert,
            ITarsConvert<byte> byteConvert, ITarsConvert<string> stringConvert,
            IDictionaryTarsConvert<string, string> dictConvert, ITarsConvert<IByteBuffer> bufferConvert,
            ITarsConvertRoot convertRoot)
        {
            this.shortConvert = shortConvert;
            this.intConvert = intConvert;
            this.byteConvert = byteConvert;
            this.stringConvert = stringConvert;
            this.dictConvert = dictConvert;
            this.bufferConvert = bufferConvert;
            this.convertRoot = convertRoot;
        }

        public override Response Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var resp = new Response();
            options.Tag = 1;
            while (buffer.IsReadable())
            {
                ReadHead(buffer, options);
                switch (options.Tag)
                {
                    case 1:
                        resp.Version = shortConvert.Deserialize(buffer, options);
                        break;

                    case 2:
                        int packetType = byteConvert.Deserialize(buffer, options);
                        break;

                    case 3:
                        resp.RequestId = intConvert.Deserialize(buffer, options);
                        break;

                    case 4:
                        resp.MessageType = intConvert.Deserialize(buffer, options);
                        break;

                    case 5:
                        resp.ResultStatusCode = (RpcStatusCode)intConvert.Deserialize(buffer, options);
                        if (resp.ResultStatusCode == RpcStatusCode.ServerSuccess)
                        {
                            ReadHead(buffer, options);
                            var contentBuffer = bufferConvert.Deserialize(buffer, options);
                            var returnType = resp.ReturnValueType.ParameterType;
                            if (returnType != typeof(void))
                            {
                                if (returnType == typeof(Task) || returnType == typeof(ValueType))
                                {
                                    resp.ReturnValue = Task.CompletedTask;
                                }
                                else
                                {
                                    ReadHead(contentBuffer, options);
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
            return resp;
        }

        public override void Serialize(Response obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            options.Tag = 1;
            shortConvert.Serialize(obj.Version, buffer, options);
            options.Tag = 2;
            byteConvert.Serialize(TarsCodecsConstant.NORMAL, buffer, options);
            options.Tag = 3;
            intConvert.Serialize(obj.RequestId, buffer, options);
            options.Tag = 4;
            intConvert.Serialize(obj.MessageType, buffer, options);
            options.Tag = 5;
            intConvert.Serialize((int)obj.ResultStatusCode, buffer, options);
            if (obj.ResultStatusCode == RpcStatusCode.ServerSuccess)
            {
                var type = obj.ReturnValueType.ParameterType;
                if (type != typeof(void) && type != typeof(Task))
                {
                    //0的位置是专门给返回值用的
                    options.Tag = 0;
                    if (type.GetTypeInfo().IsTaskWithResult())
                    {
                        convertRoot.Serialize(type.GetProperty("Result").GetReflector().GetValue(obj.ReturnValue), type.GenericTypeArguments[0], buffer, options);
                    }
                    else
                    {
                        convertRoot.Serialize(obj.ReturnValue, type, buffer, options);
                    }
                }
                int outResIndex = 0;
                foreach (var item in obj.ReturnParameterTypes)
                {
                    options.Tag = item.Position + 1;
                    convertRoot.Serialize(obj.ReturnParameters[outResIndex++], item.ParameterType, buffer, options);
                }
            }
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