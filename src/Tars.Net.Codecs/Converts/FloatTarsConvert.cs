using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class FloatTarsConvert : TarsConvertBase<float>
    {
        public FloatTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override float DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            order = tag;
            switch (tarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.FLOAT:
                    return buffer.ReadFloat();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(float obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
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