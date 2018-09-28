using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class LongTarsConvert : TarsConvertBase<long>
    {
        private readonly ITarsConvert<int> convert;
        private readonly ITarsHeadHandler headHandler;

        public LongTarsConvert(ITarsConvert<int> convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override long Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.Long:
                    return buffer.ReadLong();

                default:
                    return convert.Deserialize(buffer, options);
            }
        }

        public override void Serialize(long obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (!options.HasValue)
            {
                return;
            }
            if (obj >= int.MinValue && obj <= int.MaxValue)
            {
                convert.Serialize((int)obj, buffer, options);
            }
            else
            {
                headHandler.Reserve(buffer, 10);
                headHandler.WriteHead(buffer, TarsStructType.Long, options.Tag);
                buffer.WriteLong(obj);
            }
        }
    }
}