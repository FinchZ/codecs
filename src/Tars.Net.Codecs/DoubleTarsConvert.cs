using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        private readonly ITarsHeadHandler headHandler;

        public DoubleTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override double Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.Double:
                    return buffer.ReadDouble();

                case TarsStructType.Zero:
                    return 0D;

                default:
                    throw new TarsDecodeException($"DoubleTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(double obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (!options.HasValue)
            {
                return;
            }
            headHandler.Reserve(buffer, 10);
            if (obj == 0)
            {
                headHandler.WriteHead(buffer, TarsStructType.Zero, options.Tag);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.Double, options.Tag);
                buffer.WriteDouble(obj);
            }
        }
    }
}
