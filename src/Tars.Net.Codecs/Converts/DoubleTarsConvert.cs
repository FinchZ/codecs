using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        public DoubleTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, double value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return (tag, 0x0);

                case TarsStructBase.FLOAT:
                    return (tag, buffer.ReadFloat());

                case TarsStructBase.DOUBLE:
                    return (tag, buffer.ReadDouble());

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