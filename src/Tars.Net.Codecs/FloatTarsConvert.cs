using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class FloatTarsConvert : TarsConvertBase<float>
    {
        private readonly ITarsHeadHandler headHandler;

        public FloatTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

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
            headHandler.Reserve(buffer, 6);
            if (obj == 0)
            {
                headHandler.WriteHead(buffer, TarsStructType.ZERO_TAG, options.Tag);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.FLOAT, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteFloat(obj);
                }
            }
        }
    }
}