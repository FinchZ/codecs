using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ShortTarsConvert : TarsConvertBase<short>
    {
        public ShortTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, short value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
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

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(short obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= byte.MinValue && obj <= byte.MaxValue)
            {
                convertRoot.Serialize((byte)obj, buffer, order, isRequire, options);
            }
            else
            {
                Reserve(buffer, 4);
                WriteHead(buffer, TarsStructBase.SHORT, order);
                buffer.WriteShort(obj);
            }
        }
    }
}