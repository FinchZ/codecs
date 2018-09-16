using DotNetty.Buffers;
using System;
using System.Collections.Concurrent;

namespace Tars.Net.Codecs
{
    public class TarsConvert : ITarsConvert
    {
        private readonly ConcurrentDictionary<(Type, short), ITarsStreamConvert> converts = new ConcurrentDictionary<(Type, short), ITarsStreamConvert>();
        private readonly ITarsConvertCreatorRoot creator;

        public TarsConvert(ITarsConvertCreatorRoot creator)
        {
            this.creator = creator;
        }

        public object Deserialize(IByteBuffer buffer, Type type, TarsConvertOptions options = null)
        {
            var op = options ?? TarsConvertOptions.Default;
            return GetConvert(type, op).Deserialize(buffer, op.Encoding);
        }

        private ITarsStreamConvert GetConvert(Type type, TarsConvertOptions options)
        {
            return converts.GetOrAdd((type, options.Version), creator.Create);
        }

        public IByteBuffer Serialize(object obj, TarsConvertOptions options = null)
        {
            var op = options ?? TarsConvertOptions.Default;
            return GetConvert(obj.GetType(), op).Serialize(obj, op.Encoding);
        }
    }
}