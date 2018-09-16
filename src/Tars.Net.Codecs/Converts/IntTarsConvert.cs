using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class IntTarsConvert : TarsConvertBase<int>
    {
        public IntTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override int Deserialize(IByteBuffer buffer, TarsConvertOptions options)
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

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(int obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= short.MinValue && obj <= short.MaxValue)
            {
                convertRoot.Serialize((short)obj, buffer, options);
            }
            else
            {
                Reserve(buffer, 6);
                WriteHead(buffer, TarsStructBase.INT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteInt(obj);
                }
            }
        }
    }
}