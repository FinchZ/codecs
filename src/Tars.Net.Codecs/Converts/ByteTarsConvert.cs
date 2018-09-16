using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ByteTarsConvert : TarsConvertBase<byte>
    {
        public ByteTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, byte value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return (tag, 0x0);

                case TarsStructBase.BYTE:
                    return (tag, buffer.ReadByte());

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