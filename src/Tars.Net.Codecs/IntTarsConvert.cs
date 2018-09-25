using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class IntTarsConvert : TarsConvertBase<int>
    {
        private readonly ITarsConvert<short> convert;

        public IntTarsConvert(ITarsConvert<short> convert)
        {
            this.convert = convert;
        }

        public override int Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.INT:
                    return buffer.ReadInt();

                default:
                    return convert.Deserialize(buffer, options);
            }
        }

        public override void Serialize(int obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= short.MinValue && obj <= short.MaxValue)
            {
                convert.Serialize((short)obj, buffer, options);
            }
            else
            {
                Reserve(buffer, 6);
                WriteHead(buffer, TarsStructType.INT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteInt(obj);
                }
            }
        }
    }
}