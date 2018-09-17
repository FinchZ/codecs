using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertRoot
    {
        T Deserialize<T>(IByteBuffer buffer, TarsConvertOptions options);

        void Serialize<T>(T obj, IByteBuffer buffer, TarsConvertOptions options);
    }
}