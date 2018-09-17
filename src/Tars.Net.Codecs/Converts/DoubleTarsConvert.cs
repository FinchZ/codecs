using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        private readonly ITarsConvert<float> convert;

        public DoubleTarsConvert(ITarsConvert<float> convert)
        {
            this.convert = convert;
        }

        public override double Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.DOUBLE:
                    return buffer.ReadDouble();

                default:
                    return convert.Deserialize(buffer, options);
            }
        }

        public override void Serialize(double obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= double.MinValue && obj <= double.MaxValue)
            {
                convert.Serialize((float)obj, buffer, options);
            }
            else
            {
                Reserve(buffer, 10);
                WriteHead(buffer, TarsStructBase.DOUBLE, options.Tag);
                buffer.WriteDouble(obj);
            }
        }
    }
}