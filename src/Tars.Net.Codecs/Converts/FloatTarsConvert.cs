using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class FloatTarsConvert : TarsConvertBase<float>
    {
        public FloatTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override float Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.ZERO_TAG:
                    return 0x0;

                case TarsStructBase.FLOAT:
                    return buffer.ReadFloat();

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(float obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            Reserve(buffer, 6);
            if (obj == 0)
            {
                WriteHead(buffer, TarsStructBase.ZERO_TAG, options.Tag);
            }
            else
            {
                WriteHead(buffer, TarsStructBase.FLOAT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteFloat(obj);
                }
            }
        }
    }
}