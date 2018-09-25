using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ByteTarsConvert : TarsConvertBase<byte>
    {
        private readonly ITarsHeadHandler headHandler;

        public ByteTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override byte Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.ZERO_TAG:
                    return 0x0;

                case TarsStructType.BYTE:
                    return buffer.ReadByte();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(byte obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            headHandler.Reserve(buffer, 3);
            if (obj == 0)
            {
                headHandler.WriteHead(buffer, TarsStructType.ZERO_TAG, options.Tag);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.BYTE, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteByte(obj);
                }
            }
        }
    }
}