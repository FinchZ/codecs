using DotNetty.Buffers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tars.Net.Codecs
{
    public class TarsConvertRoot : ITarsConvertRoot
    {
        private readonly ConcurrentDictionary<(Type, short), ITarsConvert> dict = new ConcurrentDictionary<(Type, short), ITarsConvert>();
        private readonly ITarsConvert[] converts;

        public int Order => throw new NotImplementedException();

        public TarsConvertRoot(IEnumerable<ITarsConvert> converts)
        {
            this.converts = converts.OrderBy(i => i.Order).ToArray();
        }

        private ITarsConvert GetConvert(Type type, TarsConvertOptions options)
        {
            return dict.GetOrAdd((type, options.Version), (op) =>
            {
                var convert = converts.FirstOrDefault(i => i.Accept(op));
                if (convert == null)
                {
                    throw new NotSupportedException($"Codecs not supported {options}.");
                }
                return convert;
            });
        }

        public IByteBuffer Serialize(object obj, int order, bool isRequire = true, TarsConvertOptions options = null)
        {
            var op = options ?? TarsConvertOptions.Default;
            return GetConvert(obj.GetType(), op).Serialize(obj, order, isRequire, op);
        }

        public bool Accept((Type, short) options)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(IByteBuffer buffer, Type type, int order, bool isRequire = true, TarsConvertOptions options = null)
        {
            var op = options ?? TarsConvertOptions.Default;
            return GetConvert(type, op).Deserialize(buffer, type, order, isRequire, op);
        }
    }
}