using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ShortTarsConvert : TarsConvertBase<short>
    {
        private readonly ITarsConvert<byte> convert;
        private readonly ITarsHeadHandler headHandler;

        public ShortTarsConvert(ITarsConvert<byte> convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override short Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.Short:
                    return buffer.ReadShort();

                default:
                    return convert.Deserialize(buffer, options);
            }
        }

        public override void Serialize(short obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= byte.MinValue && obj <= byte.MaxValue)
            {
                convert.Serialize((byte)obj, buffer, options);
            }
            else
            {
                headHandler.Reserve(buffer, 4);
                headHandler.WriteHead(buffer, TarsStructType.Short, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteShort(obj);
                }
            }
        }
    }
}