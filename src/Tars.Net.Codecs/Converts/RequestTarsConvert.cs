using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class RequestTarsConvert : TarsConvertBase<Request>
    {
        private readonly IRpcMetadata metadata;

        public RequestTarsConvert(IServiceProvider provider) : base(provider)
        {
            metadata = provider.GetRequiredService<IRpcMetadata>();
        }

        public override Request Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var req = new Request();
            options.Tag = 1;
            while (options.Tag != 0 && buffer.IsReadable())
            {
                var (tarsType, tag, tagType) = convertRoot.ReadHead(buffer);
                options.Tag = tag;
                options.TarsType = tarsType;
                switch (tag)
                {
                    case 1:
                        req.Version = convertRoot.Deserialize<short>(buffer, options);
                        break;
                    case 2:
                        req.PacketType = convertRoot.Deserialize<byte>(buffer, options);
                        break;
                    case 3:
                        req.MessageType = convertRoot.Deserialize<int>(buffer, options);
                        break;
                    case 4:
                        req.RequestId = convertRoot.Deserialize<int>(buffer, options);
                        break;
                    case 5:
                        req.ServantName = convertRoot.Deserialize<string>(buffer, options);
                        break;
                    case 6:
                        req.FuncName = convertRoot.Deserialize<string>(buffer, options);
                        break;
                    case 7:
                        //// todo : use metadata to Deserialize content
                        ////req.Buffer = stream.ReadByteArray(7);//数据
                        break;
                    case 8:
                        req.Timeout = convertRoot.Deserialize<int>(buffer, options);
                        break;
                    case 9:
                        //req.Context = convertRoot.Deserialize<Dictionary<string, string>>(buffer, options);
                        break;
                    case 10:
                        //req.Status = convertRoot.Deserialize<Dictionary<string, string>>(buffer, options);
                        break;
                    default:
                        break;
                }
            }
            return req;
        }

        public override void Serialize(Request obj, IByteBuffer buffer, int order, TarsConvertOptions options)
        {
            convertRoot.Serialize(obj.Version, buffer, 1, options);
            convertRoot.Serialize(obj.PacketType, buffer, 2, options);
            convertRoot.Serialize(obj.MessageType, buffer, 3, options);
            convertRoot.Serialize(obj.RequestId, buffer, 4, options);
            convertRoot.Serialize(obj.ServantName, buffer, 5, options);
            convertRoot.Serialize(obj.FuncName, buffer, 6, options);
            // todo : use metadata to Serialize content
            //stream.Write(EncodeRequestContent(req, charsetName), 7);
            convertRoot.Serialize(obj.Timeout, buffer, 8, options);
            convertRoot.Serialize(obj.Context, buffer, 9, options);
            convertRoot.Serialize(obj.Status, buffer, 10, options);
        }
    }
}