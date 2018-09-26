using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class IntTarsConvert : TarsConvertBase<int>
    {
        private readonly ITarsConvert<short> convert;
        private readonly ITarsHeadHandler headHandler;

        public IntTarsConvert(ITarsConvert<short> convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override int Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.Int:
                    return buffer.ReadInt();

                default:
                    return convert.Deserialize(buffer, options);
            }
        }

        public override void Serialize(int obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj >= short.MinValue && obj <= short.MaxValue)
            {
                convert.Serialize((short)obj, buffer, options);
            }
            else
            {
                headHandler.Reserve(buffer, 6);
                headHandler.WriteHead(buffer, TarsStructType.Int, options.Tag);
                if (options.HasValue)
                {
                    buffer.WriteInt(obj);
                }
            }
        }
    }
}