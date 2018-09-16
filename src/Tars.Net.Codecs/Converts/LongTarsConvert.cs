using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class LongTarsConvert : TarsConvertBase<long>
    {
        public LongTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override long DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            order = tag;
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.BYTE:
                    return buffer.ReadByte();

                case TarsStructBase.SHORT:
                    return buffer.ReadShort();

                case TarsStructBase.INT:
                    return buffer.ReadInt();

                case TarsStructBase.LONG:
                    return buffer.ReadLong();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(long obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= int.MinValue && obj <= int.MaxValue)
            {
                convertRoot.Serialize((int)obj, order);
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