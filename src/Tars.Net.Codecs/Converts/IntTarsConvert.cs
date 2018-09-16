using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class IntTarsConvert : TarsConvertBase<int>
    {
        public IntTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, int value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
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

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(int obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= short.MinValue && obj <= short.MaxValue)
            {
                convertRoot.Serialize((short)obj, buffer, order, isRequire, options);
            }
            else
            {
                Reserve(buffer, 6);
                WriteHead(buffer, TarsStructBase.INT, order);
                buffer.WriteInt(obj);
            }
        }
    }
}