using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public interface ICanTarsConvert
    {
        bool Accept(Codec codec, short version, Type type);
    }

    public interface ITarsConvert<T> : ICanTarsConvert
    {
        T Deserialize(IByteBuffer buffer, TarsConvertOptions options);

        void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options);
    }
}