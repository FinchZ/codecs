using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ByteTarsConvert : TarsConvertBase<byte>
    {
        public ByteTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override byte Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.BYTE:
                    return buffer.ReadByte();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(byte obj, IByteBuffer buffer, int order, TarsConvertOptions options)
        {
            Reserve(buffer, 3);
            if (obj == 0)
            {
                WriteHead(buffer, TarsStructBase.ZERO_TAG, order);
            }
            else
            {
                WriteHead(buffer, TarsStructBase.BYTE, order);
                buffer.WriteByte(obj);
            }
        }
    }
}