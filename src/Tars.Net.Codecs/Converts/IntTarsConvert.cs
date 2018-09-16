using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs.Converts
{
    public class IntTarsConvert : TarsConvertBase<int>
    {
        public IntTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override int DeserializeT(IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
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

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(int obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (obj >= short.MinValue && obj <= short.MaxValue)
            {
                convertRoot.Serialize((short)obj, order);
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