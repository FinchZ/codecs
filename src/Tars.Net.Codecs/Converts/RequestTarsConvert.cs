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
            var tag = options.Tag = 1;
            while (tag != 0 && buffer.IsReadable())
            {
                convertRoot.ReadHead(buffer, options);
                tag = options.Tag;
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
                        convertRoot.ReadHead(buffer, options);
                        var contentBuffer = convertRoot.Deserialize<IByteBuffer>(buffer, options);
                        req.Parameters = new object[req.ParameterTypes.Length];
                        while (contentBuffer.IsReadable())
                        {
                            convertRoot.ReadHead(buffer, options);
                            //req.Parameters[options.Tag] = convertRoot.Deserialize<>(contentBuffer, options);
                        }
                        break;
                    case 8:
                        req.Timeout = convertRoot.Deserialize<int>(buffer, options);
                        break;
                    case 9:
                        req.Context = convertRoot.Deserialize<IDictionary<string, string>>(buffer, options);
                        break;
                    case 10:
                        req.Status = convertRoot.Deserialize<IDictionary<string, string>>(buffer, options);
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
            convertRoot.Serialize(obj.Version, buffer, options);
            options.Tag = 2;
            convertRoot.Serialize(obj.PacketType, buffer, options);
            options.Tag = 3;
            convertRoot.Serialize(obj.MessageType, buffer, options);
            options.Tag = 4;
            convertRoot.Serialize(obj.RequestId, buffer, options);
            options.Tag = 5;
            convertRoot.Serialize(obj.ServantName, buffer, options);
            options.Tag = 6;
            convertRoot.Serialize(obj.FuncName, buffer, options);

            // todo : use metadata to Serialize content
            var contentBuffer = Unpooled.Buffer(128);
            for (int i = 0; i < obj.ParameterTypes.Length; i++)
            {
                options.Tag = obj.ParameterTypes[i].Position;
                //convertRoot.Serialize<>(obj.Parameters[i], contentBuffer, options);
            }
            
            options.Tag = 7;
            convertRoot.Serialize(contentBuffer, buffer, options);
            options.Tag = 8;
            convertRoot.Serialize(obj.Timeout, buffer, options);
            options.Tag = 9;
            convertRoot.Serialize(obj.Context, buffer, options);
            options.Tag = 10;
            convertRoot.Serialize(obj.Status, buffer, options);
        }
    }
}