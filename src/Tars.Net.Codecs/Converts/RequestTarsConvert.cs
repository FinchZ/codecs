using DotNetty.Buffers;
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

        public RequestTarsConvert(ITarsConvert<short> shortConvert, ITarsConvert<int> intConvert,
            ITarsConvert<byte> byteConvert, ITarsConvert<string> stringConvert,
            IDictionaryTarsConvert<string, string> dictConvert, ITarsConvert<IByteBuffer> bufferConvert)
        {
            this.shortConvert = shortConvert;
            this.intConvert = intConvert;
            this.byteConvert = byteConvert;
            this.stringConvert = stringConvert;
            this.dictConvert = dictConvert;
            this.bufferConvert = bufferConvert;
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
                        break;

                    case 7:
                        ReadHead(buffer, options);
                        req.Buffer = bufferConvert.Deserialize(buffer, options);
                        //// todo : use metadata to Deserialize content
                        //req.Parameters = new object[req.ParameterTypes.Length];
                        //while (contentBuffer.IsReadable())
                        //{
                        //    convertRoot.ReadHead(buffer, options);
                        //    //req.Parameters[options.Tag] = convertRoot.Deserialize<>(contentBuffer, options);
                        //}
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

            // todo : use metadata to Serialize content
            //var contentBuffer = Unpooled.Buffer(128);
            //for (int i = 0; i < obj.ParameterTypes.Length; i++)
            //{
            //    options.Tag = obj.ParameterTypes[i].Position;
            //    //convertRoot.Serialize<>(obj.Parameters[i], contentBuffer, options);
            //}

            options.Tag = 7;
            bufferConvert.Serialize((IByteBuffer)obj.Buffer, buffer, options);
            options.Tag = 8;
            intConvert.Serialize(obj.Timeout, buffer, options);
            options.Tag = 9;
            dictConvert.Serialize(obj.Context, buffer, options);
            options.Tag = 10;
            dictConvert.Serialize(obj.Status, buffer, options);
        }
    }
}