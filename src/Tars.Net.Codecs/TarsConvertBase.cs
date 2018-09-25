using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public abstract class TarsConvertBase<T> : ITarsConvert<T>
    {
        public virtual bool Accept(Codec codec, short version, Type type)
        {
            return codec == Codec.Tars && type == typeof(T);
        }

        public abstract T Deserialize(IByteBuffer buffer, TarsConvertOptions options);

        public abstract void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options);
    }
}