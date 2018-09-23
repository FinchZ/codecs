using DotNetty.Buffers;
using System.Threading.Tasks;

namespace Tars.Net.Codecs
{
    public interface ITaskTarsConvert<T> : ITarsConvert<Task<T>>
    { }

    public class TaskTarsConvert<T> : TarsConvertBase<Task<T>>, ITaskTarsConvert<T>
    {
        private readonly ITarsConvert<T> convert;

        public TaskTarsConvert(ITarsConvert<T> convert)
        {
            this.convert = convert;
        }

        public override Task<T> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return Task.FromResult(convert.Deserialize(buffer, options));
        }

        public override void Serialize(Task<T> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convert.Serialize(obj.Result, buffer, options);
        }
    }
}