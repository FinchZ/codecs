using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        private readonly ITarsConvert<float> convert;
        private readonly ITarsHeadHandler headHandler;

        public DoubleTarsConvert(ITarsConvert<float> convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override double Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.DOUBLE:
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
                headHandler.Reserve(buffer, 10);
                headHandler.WriteHead(buffer, TarsStructType.DOUBLE, options.Tag);
                buffer.WriteDouble(obj);
            }
        }
    }
}