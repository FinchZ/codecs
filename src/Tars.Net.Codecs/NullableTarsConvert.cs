using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class NullableTarsConvert<T> : TarsConvertBase<T?> where T : struct
    {
        private readonly ITarsConvert<T> convert;

        public NullableTarsConvert(ITarsConvert<T> convert)
        {
            this.convert = convert;
        }

        public override T? Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var old = options.HasValue;
            options.HasValue = false;
            var value = convert.Deserialize(buffer, options);
            options.HasValue = old;
            return options.HasValue ? (T?)value : null;
        }

        public override void Serialize(T? obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            var old = options.HasValue;
            options.HasValue = obj.HasValue;
            convert.Serialize(obj.GetValueOrDefault(), buffer, options);
            options.HasValue = old;
        }
    }
}