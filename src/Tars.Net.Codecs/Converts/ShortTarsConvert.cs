using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ShortTarsConvert : TarsConvertBase<short>
    {
        public ShortTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override short DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
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

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(short obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= byte.MinValue && obj <= byte.MaxValue)
            {
                convertRoot.Serialize((byte)obj, order);
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