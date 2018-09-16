using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class NullableTarsConvert<T> : TarsConvertBase<T?> where T : struct
    {
        public NullableTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override T? Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var old = options.HasValue;
            options.HasValue = false;
            var value = convertRoot.Deserialize<T>(buffer, options);
            options.HasValue = old;
            return options.HasValue ? (T?)value : null;
        }

        public override void Serialize(T? obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            var old = options.HasValue;
            options.HasValue = obj.HasValue;
            convertRoot.Serialize(obj.GetValueOrDefault(), buffer, options);
            options.HasValue = old;
        }
    }
}