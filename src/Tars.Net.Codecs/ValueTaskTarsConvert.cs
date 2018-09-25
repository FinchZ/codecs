using DotNetty.Buffers;
using System.Threading.Tasks;

namespace Tars.Net.Codecs
{
    public interface IValueTaskTarsConvert<T> : ITarsConvert<ValueTask<T>>
    { }

    public class ValueTaskTarsConvert<T> : TarsConvertBase<ValueTask<T>>, IValueTaskTarsConvert<T>
    {
        private readonly ITarsConvert<T> convert;

        public ValueTaskTarsConvert(ITarsConvert<T> convert)
        {
            this.convert = convert;
        }

        public override ValueTask<T> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return new ValueTask<T>(convert.Deserialize(buffer, options));
        }

        public override void Serialize(ValueTask<T> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convert.Serialize(obj.Result, buffer, options);
        }
    }
}