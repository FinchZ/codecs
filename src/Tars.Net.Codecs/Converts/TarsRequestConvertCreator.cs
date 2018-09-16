using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using Tars.Net.Codecs.Exceptions;
using Tars.Net.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Codecs
{
    public class TarsRequestConvertCreator : ITarsConvertCreator
    {
        private readonly IServiceProvider provider;

        public int Order => 0;

        public TarsRequestConvertCreator(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public bool Accept((Type, short) options)
        {
            return options.Item1 == typeof(Request);
        }

        public ITarsStreamConvert Create((Type, short) options)
        {
            return new CustomTarsStreamConvert(CreateDeserialize(), CreateSerialize());
        }

        private Func<IByteBuffer, Encoding, object> CreateDeserialize()
        {
            var metadata = provider.GetRequiredService<IRpcMetadata>();
            return (buffer, encoding) =>
            {
                var req = new Request();
                var stream = new TarsInputStream(buffer);
                stream.SetEncoding(encoding);
                req.Version = stream.ReadShort(1, true);
                req.PacketType = stream.ReadByte(2, true);
                req.MessageType = stream.ReadInt(3, true);
                req.RequestId = stream.ReadInt(4, true);
                req.ServantName = stream.ReadString(5, true);
                req.FuncName = stream.ReadString(6, true);

                // todo : use metadata to Deserialize content
                //req.Buffer = stream.ReadByteArray(7, true);//数据
                req.Timeout = stream.ReadInt(8, true);//超时时间
                req.Context = stream.ReadMap(9, true);
                req.Status = stream.ReadMap(10, true);
                return req;
            };
        }

        private Func<object, Encoding, IByteBuffer> CreateSerialize()
        {
            var metadata = provider.GetRequiredService<IRpcMetadata>();
            return (obj, encoding) =>
            {
                var stream = new TarsOutputStream();
                var req = obj as Request; 
                stream.SetEncoding(encoding);
                stream.GetByteBuffer().WriteInt(0);
                stream.Write(req.Version, 1);
                stream.Write(req.PacketType, 2);
                stream.Write(req.MessageType, 3);
                stream.Write(req.RequestId, 4);
                stream.Write(req.ServantName, 5);
                stream.Write(req.FuncName, 6);
                // todo : use metadata to Serialize content
                //stream.Write(EncodeRequestContent(req, charsetName), 7);
                stream.Write(req.Timeout, 8);
                stream.Write(req.Context, 9);
                stream.Write(req.Status, 10);
                IByteBuffer buffer = stream.GetByteBuffer();
                int length = buffer.WriterIndex;
                if (length > TarsCodecsConstant.PACKAGE_MAX_LENGTH || length <= 0)
                    throw new ProtocolException("the length header of the package must be between 0~10M bytes. data length:" + length);
                stream.ResetDataLength(length);
                return Unpooled.WrappedBuffer(stream.ToByteArray());
            };
        }
    }
}
