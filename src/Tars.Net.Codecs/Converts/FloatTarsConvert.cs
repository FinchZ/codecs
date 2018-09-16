using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class FloatTarsConvert : TarsConvertBase<float>
    {
        public FloatTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, float value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return (tag, 0x0);

                case TarsStructBase.FLOAT:
                    return (tag, buffer.ReadFloat());

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(float obj, IByteBuffer buffer, int order, TarsConvertOptions options)
        {
            Reserve(buffer, 6);
            if (obj == 0)
            {
                WriteHead(buffer, TarsStructBase.ZERO_TAG, order);
            }
            else
            {
                WriteHead(buffer, TarsStructBase.FLOAT, order);
                buffer.WriteFloat(obj);
            }
        }
    }
}