using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        public DoubleTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override double Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.FLOAT:
                    return buffer.ReadFloat();

                case TarsStructBase.DOUBLE:
                    return buffer.ReadDouble();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(double obj, IByteBuffer buffer, int order, TarsConvertOptions options)
        {
            if (obj >= double.MinValue && obj <= double.MaxValue)
            {
                convertRoot.Serialize((float)obj, buffer, order, options);
            }
            else
            {
                Reserve(buffer, 10);
                WriteHead(buffer, TarsStructBase.DOUBLE, order);
                buffer.WriteDouble(obj);
            }
        }
    }
}