using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ByteBufferTarsConvert : TarsConvertBase<IByteBuffer>
    {
        private readonly ITarsHeadHandler headHandler;

        public ByteBufferTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override IByteBuffer Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.ByteArray:
                    headHandler.ReadHead(buffer, options);
                    int size = buffer.ReadInt();
                    return buffer.ReadBytes(size);

                default:
                    throw new TarsDecodeException($"ByteBufferTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(IByteBuffer obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj == null)
            {
                return;
            }

            int len = obj.ReadableBytes;
            headHandler.Reserve(buffer, 8 + len);
            headHandler.WriteHead(buffer, TarsStructType.ByteArray, options.Tag);
            headHandler.WriteHead(buffer, TarsStructType.Byte, 0);
            buffer.WriteInt(len);
            buffer.WriteBytes(obj);
        }
    }
}