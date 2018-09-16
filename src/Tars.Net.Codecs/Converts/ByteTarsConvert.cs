using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class ByteTarsConvert : TarsConvertBase<byte>
    {
        public ByteTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override byte DeserializeT(IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.BYTE:
                    return buffer.ReadByte();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(byte obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
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