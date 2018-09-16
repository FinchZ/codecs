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

        public override Request DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
        {
            order = 0;
            var req = new Request
            {
                //Version = convertRoot.Deserialize<short>(buffer, 1, true, options),
                //PacketType = convertRoot.Deserialize<byte>(buffer, 2, true, options),
                //MessageType = convertRoot.Deserialize<int>(buffer, 3, true, options),
                //RequestId = convertRoot.Deserialize<int>(buffer, 4, true, options),
                //ServantName = convertRoot.Deserialize<string>(buffer, 5, true, options),
                //FuncName = convertRoot.Deserialize<string>(buffer, 6, true, options),

                //// todo : use metadata to Deserialize content
                ////req.Buffer = stream.ReadByteArray(7, true);//数据
                //Timeout = convertRoot.Deserialize<int>(buffer, 8, true, options),
                //Context = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 9, true, options),
                //Status = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 10, true, options)
            };
            return req;
        }

        public override void SerializeT(Request obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            convertRoot.Serialize(obj.Version, buffer, 1, true, options);
            convertRoot.Serialize(obj.PacketType, buffer, 2, true, options);
            convertRoot.Serialize(obj.MessageType, buffer, 3, true, options);
            convertRoot.Serialize(obj.RequestId, buffer, 4, true, options);
            convertRoot.Serialize(obj.ServantName, buffer, 5, true, options);
            convertRoot.Serialize(obj.FuncName, buffer, 6, true, options);
            // todo : use metadata to Serialize content
            //stream.Write(EncodeRequestContent(req, charsetName), 7);
            convertRoot.Serialize(obj.Timeout, buffer, 8, true, options);
            convertRoot.Serialize(obj.Context, buffer, 9, true, options);
            convertRoot.Serialize(obj.Status, buffer, 10, true, options);
        }
    }
}