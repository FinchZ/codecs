using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class DoubleTarsConvert : TarsConvertBase<double>
    {
        public DoubleTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override double DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            order = tag;
            switch (tarsType)
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

        public override void SerializeT(double obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= double.MinValue && obj <= double.MaxValue)
            {
                convertRoot.Serialize((float)obj, order);
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