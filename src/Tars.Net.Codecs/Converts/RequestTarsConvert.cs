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

        public override (int order, Request value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var req = new Request
            {
                //Version = convertRoot.Deserialize<short>(buffer, 1, options),
                //PacketType = convertRoot.Deserialize<byte>(buffer, 2, options),
                //MessageType = convertRoot.Deserialize<int>(buffer, 3, options),
                //RequestId = convertRoot.Deserialize<int>(buffer, 4, options),
                //ServantName = convertRoot.Deserialize<string>(buffer, 5, options),
                //FuncName = convertRoot.Deserialize<string>(buffer, 6, options),

                //// todo : use metadata to Deserialize content
                ////req.Buffer = stream.ReadByteArray(7);//数据
                //Timeout = convertRoot.Deserialize<int>(buffer, 8, options),
                //Context = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 9, options),
                //Status = convertRoot.Deserialize<Dictionary<string, string>>(buffer, 10, options)
            };
            return (0, req);
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