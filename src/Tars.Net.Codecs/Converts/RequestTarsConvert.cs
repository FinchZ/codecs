using DotNetty.Buffers;
using System.Collections.Generic;
using System.Linq;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class RequestTarsConvert : TarsConvertBase<Request>
    {
        private readonly ITarsConvert<short> shortConvert;
        private readonly ITarsConvert<int> intConvert;
        private readonly ITarsConvert<byte> byteConvert;
        private readonly ITarsConvert<string> stringConvert;
        private readonly IDictionaryTarsConvert<string, string> dictConvert;
        private readonly ITarsConvert<IByteBuffer> bufferConvert;
        private readonly ITarsConvertRoot convertRoot;
        private readonly IRpcMetadata rpcMetadata;

        public RequestTarsConvert(ITarsConvert<short> shortConvert, ITarsConvert<int> intConvert,
            ITarsConvert<byte> byteConvert, ITarsConvert<string> stringConvert,
            IDictionaryTarsConvert<string, string> dictConvert, ITarsConvert<IByteBuffer> bufferConvert,
            ITarsConvertRoot convertRoot, IRpcMetadata rpcMetadata)
        {
            this.shortConvert = shortConvert;
            this.intConvert = intConvert;
            this.byteConvert = byteConvert;
            this.stringConvert = stringConvert;
            this.dictConvert = dictConvert;
            this.bufferConvert = bufferConvert;
            this.convertRoot = convertRoot;
            this.rpcMetadata = rpcMetadata;
        }

        public override Request Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var req = new Request();
            var tag = options.Tag = 1;
            while (tag != 0 && buffer.IsReadable())
            {
                ReadHead(buffer, options);
                tag = options.Tag;
                switch (tag)
                {
                    case 1:
                        req.Version = shortConvert.Deserialize(buffer, options);
                        options.Version = req.Version;
                        break;

                    case 2:
                        req.PacketType = byteConvert.Deserialize(buffer, options);
                        break;

                    case 3:
                        req.MessageType = intConvert.Deserialize(buffer, options);
                        break;

                    case 4:
                        req.RequestId = intConvert.Deserialize(buffer, options);
                        break;

                    case 5:
                        req.ServantName = stringConvert.Deserialize(buffer, options);
                        break;

                    case 6:
                        req.FuncName = stringConvert.Deserialize(buffer, options);
                        var (method, isOneway, outParameters, codec, version, serviceType) = rpcMetadata.FindRpcMethod(req.ServantName, req.FuncName);
                        req.IsOneway = isOneway;
                        req.Mehtod = method;
                        req.ReturnParameterTypes = outParameters;
                        req.ServiceType = serviceType;
                        req.ParameterTypes = method.GetParameters();
                        break;

                    case 7 when options.Version == TarsCodecsVersion.V1:
                        {
                            var contentBuffer = bufferConvert.Deserialize(buffer, options);
                            req.Parameters = new object[req.ParameterTypes.Length];
                            while (contentBuffer.IsReadable())
                            {
                                ReadHead(contentBuffer, options);
                                var index = options.Tag - 1;
                                var type = req.ParameterTypes[index];
                                req.Parameters[index] = convertRoot.Deserialize(contentBuffer, type.ParameterType, options);
                            }
                        }
                        break;

                    case 7 when options.Version == TarsCodecsVersion.V2:
                        {
                            var contentBuffer = bufferConvert.Deserialize(buffer, options);
                            var uni = new UniAttributeV2(convertRoot);
                            ReadHead(contentBuffer, options);
                            uni.Deserialize(contentBuffer, options);
                            req.Parameters = new object[req.ParameterTypes.Length];
                            foreach (var pt in req.ParameterTypes)
                            {
                                var pBuffer = uni.Temp[pt.Name].Values.First();
                                ReadHead(pBuffer, options);
                                req.Parameters[pt.Position] = convertRoot.Deserialize(pBuffer, pt.ParameterType, options);
                            }
                        }
                        break;

                    case 7 when options.Version == TarsCodecsVersion.V3:
                        {
                            var contentBuffer = bufferConvert.Deserialize(buffer, options);
                            var uni = new UniAttributeV3(convertRoot);
                            ReadHead(contentBuffer, options);
                            uni.Deserialize(contentBuffer, options);
                            req.Parameters = new object[req.ParameterTypes.Length];
                            foreach (var pt in req.ParameterTypes)
                            {
                                var pBuffer = uni.Temp[pt.Name];
                                ReadHead(pBuffer, options);
                                req.Parameters[pt.Position] = convertRoot.Deserialize(pBuffer, pt.ParameterType, options);
                            }
                        }
                        break;

                    case 8:
                        req.Timeout = intConvert.Deserialize(buffer, options);
                        break;

                    case 9:
                        req.Context = dictConvert.Deserialize(buffer, options);
                        break;

                    case 10:
                        req.Status = dictConvert.Deserialize(buffer, options);
                        tag = 0;
                        break;

                    default:
                        break;
                }
            }
            return req;
        }

        public override void Serialize(Request obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            options.Tag = 1;
            options.Version = obj.Version;
            // for test
            obj.Version = options.Version = 0x03;
            shortConvert.Serialize(obj.Version, buffer, options);
            options.Tag = 2;
            byteConvert.Serialize(obj.PacketType, buffer, options);
            options.Tag = 3;
            intConvert.Serialize(obj.MessageType, buffer, options);
            options.Tag = 4;
            intConvert.Serialize(obj.RequestId, buffer, options);
            options.Tag = 5;
            stringConvert.Serialize(obj.ServantName, buffer, options);
            options.Tag = 6;
            stringConvert.Serialize(obj.FuncName, buffer, options);

            switch (options.Version)
            {
                case TarsCodecsVersion.V3:
                    {
                        var uni = new UniAttributeV3(convertRoot)
                        {
                            Temp = new Dictionary<string, IByteBuffer>(obj.ParameterTypes.Length)
                        };
                        for (int i = 0; i < obj.ParameterTypes.Length; i++)
                        {
                            var p = obj.ParameterTypes[i];
                            uni.Put(p.Name, obj.Parameters[i], p.ParameterType, options);
                        }

                        options.Tag = 7;
                        uni.Serialize(buffer, options);
                    }
                    break;

                case TarsCodecsVersion.V2:
                    {
                        var uni = new UniAttributeV2(convertRoot)
                        {
                            Temp = new Dictionary<string, IDictionary<string, IByteBuffer>>(obj.ParameterTypes.Length)
                        };
                        for (int i = 0; i < obj.ParameterTypes.Length; i++)
                        {
                            var p = obj.ParameterTypes[i];
                            uni.Put(p.Name, obj.Parameters[i], p.ParameterType, options);
                        }

                        options.Tag = 7;
                        uni.Serialize(buffer, options);
                    }
                    break;

                default:
                    {
                        var contentBuffer = Unpooled.Buffer(128);
                        for (int i = 0; i < obj.ParameterTypes.Length; i++)
                        {
                            var p = obj.ParameterTypes[i];
                            options.Tag = p.Position + 1;
                            convertRoot.Serialize(obj.Parameters[i], p.ParameterType, contentBuffer, options);
                        }

                        options.Tag = 7;
                        bufferConvert.Serialize(contentBuffer, buffer, options);
                    }
                    break;
            }

            options.Tag = 8;
            intConvert.Serialize(obj.Timeout, buffer, options);
            options.Tag = 9;
            dictConvert.Serialize(obj.Context, buffer, options);
            options.Tag = 10;
            dictConvert.Serialize(obj.Status, buffer, options);
        }
    }
}