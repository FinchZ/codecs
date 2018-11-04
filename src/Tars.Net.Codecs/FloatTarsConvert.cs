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
                case TarsStructType.Zero:
                    return 0F;

                case TarsStructType.Float:
                    return buffer.ReadFloat();

                default:
                    throw new TarsDecodeException($"FloatTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(float obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (!options.HasValue)
            {
                return;
            }
            headHandler.Reserve(buffer, 6);
            if (obj == 0)
            {
                headHandler.WriteHead(buffer, TarsStructType.Zero, options.Tag);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.Float, options.Tag);
                buffer.WriteFloat(obj);
            }
        }
    }
}
