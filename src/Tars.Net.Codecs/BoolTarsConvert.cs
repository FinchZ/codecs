using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class BoolTarsConvert : TarsConvertBase<bool>
    {
        private readonly ITarsConvert<byte> convert;

        public BoolTarsConvert(ITarsConvert<byte> convert)
        {
            this.convert = convert;
        }

        public override bool Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var value = convert.Deserialize(buffer, options);
            return value != 0;
        }

        public override void Serialize(bool obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convert.Serialize((byte)(obj ? 0x01 : 0), buffer, options);
        }
    }
}