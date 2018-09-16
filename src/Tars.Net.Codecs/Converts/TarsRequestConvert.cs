using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsRequestConvert : TarsConvertBase<Request>
    {
        private readonly IRpcMetadata metadata;

        public TarsRequestConvert(IServiceProvider provider) : base(provider)
        {
            metadata = provider.GetRequiredService<IRpcMetadata>();
        }

        public override bool AcceptVersion(short version)
        {
            return true;
        }

        public override Request DeserializeT(IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            var req = new Request
            {
                Version = convertRoot.Deserialize<short>(buffer, 1, true, options),
                PacketType = convertRoot.Deserialize<byte>(buffer, 2, true, options),
                MessageType = convertRoot.Deserialize<int>(buffer, 3, true, options),
                RequestId = convertRoot.Deserialize<int>(buffer, 4, true, options),
                ServantName = convertRoot.Deserialize<string>(buffer, 5, true, options),
                FuncName = convertRoot.Deserialize<string>(buffer, 6, true, options),

                // todo : use metadata to Deserialize content
                //req.Buffer = stream.ReadByteArray(7, true);//数据
                Timeout = convertRoot.Deserialize<int>(buffer, 8, true, options),
                Context = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 9, true, options),
                Status = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 10, true, options)
            };
            return req;
        }

        public override IByteBuffer SerializeT(Request obj, int order, bool isRequire, TarsConvertOptions options)
        {
            var buffer = Unpooled.Buffer(128);
            convertRoot.Serialize(obj.Version, 1, true, options);
            convertRoot.Serialize(obj.PacketType, 2, true, options);
            convertRoot.Serialize(obj.MessageType, 3, true, options);
            convertRoot.Serialize(obj.RequestId, 4, true, options);
            convertRoot.Serialize(obj.ServantName, 5, true, options);
            convertRoot.Serialize(obj.FuncName, 6, true, options);
            // todo : use metadata to Serialize content
            //stream.Write(EncodeRequestContent(req, charsetName), 7);
            convertRoot.Serialize(obj.Timeout, 8, true, options);
            convertRoot.Serialize(obj.Context, 9, true, options);
            convertRoot.Serialize(obj.Status, 10, true, options);
            return buffer;
        }
    }
}