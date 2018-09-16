using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ShortTarsConvert : TarsConvertBase<short>
    {
        public ShortTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override short Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
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

        public override void Serialize(short obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= byte.MinValue && obj <= byte.MaxValue)
            {
                convertRoot.Serialize((byte)obj, buffer, options);
            }
            else
            {
                Reserve(buffer, 4);
                WriteHead(buffer, TarsStructBase.SHORT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteShort(obj);
                }
            }
        }
    }
}