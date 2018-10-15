using DotNetty.Buffers;
using System;
using System.Threading.Tasks;

namespace Tars.Net.Codecs
{
    public interface ITaskTarsConvert<T> : ITarsConvert<Task<T>>
    { }

    public class TaskResultTarsConvert<T> : TarsConvertBase<Task<T>>, ITaskTarsConvert<T>
    {
        private readonly ITarsConvert<T> convert;

        public TaskResultTarsConvert(ITarsConvert<T> convert)
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

    public class TaskTarsConvert : TarsConvertBase<Task>
    {
        public override Task Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return Task.CompletedTask;
        }

        public override void Serialize(Task obj, IByteBuffer buffer, TarsConvertOptions options)
        {
        }
    }
}