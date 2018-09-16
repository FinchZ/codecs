using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public interface ICanTarsConvert
    {
        bool Accept(Codec codec, short version);
    }

    public interface ITarsConvert<T> : ICanTarsConvert
    {
        (int order, T value) Deserialize(IByteBuffer buffer, TarsConvertOptions options);

        void Serialize(T obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options);
    }
}