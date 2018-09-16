using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public interface ITarsConvertRoot 
    {
        (int order, T value) Deserialize<T>(IByteBuffer buffer, TarsConvertOptions options);

        void Serialize<T>(T obj, IByteBuffer buffer, int order, TarsConvertOptions options);
    }
}