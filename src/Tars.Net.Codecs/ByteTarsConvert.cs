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
                case TarsStructType.Zero:
                    return 0x0;

                case TarsStructType.Byte:
                    return buffer.ReadByte();

                default:
                    throw new TarsDecodeException($"ByteTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(byte obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (!options.HasValue)
            {
                return;
            }

            headHandler.Reserve(buffer, 3);
            if (obj == 0)
            {
                headHandler.WriteHead(buffer, TarsStructType.Zero, options.Tag);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.Byte, options.Tag);
                buffer.WriteByte(obj);
            }
        }
    }
}