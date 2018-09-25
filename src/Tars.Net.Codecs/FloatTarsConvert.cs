using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class FloatTarsConvert : TarsConvertBase<float>
    {
        public override float Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.ZERO_TAG:
                    return 0x0;

                case TarsStructType.FLOAT:
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
                WriteHead(buffer, TarsStructType.ZERO_TAG, options.Tag);
            }
            else
            {
                WriteHead(buffer, TarsStructType.FLOAT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteFloat(obj);
                }
            }
        }
    }
}