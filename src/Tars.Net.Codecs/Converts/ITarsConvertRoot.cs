using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertRoot
    {
        T Deserialize<T>(IByteBuffer buffer, TarsConvertOptions options);

        void Serialize<T>(T obj, IByteBuffer buffer, TarsConvertOptions options);

        void Serialize(object obj, Type type, IByteBuffer buffer, TarsConvertOptions options);

        object Deserialize(IByteBuffer buffer, Type type, TarsConvertOptions options);
    }
}