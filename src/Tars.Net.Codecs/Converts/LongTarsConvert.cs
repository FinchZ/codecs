using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class LongTarsConvert : TarsConvertBase<long>
    {
        public LongTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override long Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
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

        public override void Serialize(long obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= int.MinValue && obj <= int.MaxValue)
            {
                convertRoot.Serialize((int)obj, buffer, options);
            }
            else
            {
                Reserve(buffer, 10);
                WriteHead(buffer, TarsStructBase.LONG, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteLong(obj);
                }
            }
        }
    }
}