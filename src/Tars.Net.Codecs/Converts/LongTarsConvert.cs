using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class LongTarsConvert : TarsConvertBase<long>
    {
        public LongTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, long value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return (tag, 0x0);

                case TarsStructBase.BYTE:
                    return (tag, buffer.ReadByte());

                case TarsStructBase.SHORT:
                    return (tag, buffer.ReadShort());

                case TarsStructBase.INT:
                    return (tag, buffer.ReadInt());

                case TarsStructBase.LONG:
                    return (tag, buffer.ReadLong());

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(long obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= int.MinValue && obj <= int.MaxValue)
            {
                convertRoot.Serialize((int)obj, buffer, order, isRequire, options);
            }
            else
            {
                Reserve(buffer, 10);
                WriteHead(buffer, TarsStructBase.LONG, order);
                buffer.WriteLong(obj);
            }
        }
    }
}