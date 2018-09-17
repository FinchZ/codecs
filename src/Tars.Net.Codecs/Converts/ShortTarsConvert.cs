using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ShortTarsConvert : TarsConvertBase<short>
    {
        private readonly ITarsConvert<byte> convert;

        public ShortTarsConvert(ITarsConvert<byte> convert)
        {
            this.convert = convert;
        }

        public override short Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.SHORT:
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
                Reserve(buffer, 4);
                WriteHead(buffer, TarsStructBase.SHORT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteShort(obj);
                }
            }
        }
    }
}