using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvert
    {
        int Order { get; }

        bool Accept((Type, short) options);

        object Deserialize(IByteBuffer buffer, Type type, int order, bool isRequire = true, TarsConvertOptions options = null);

        void Serialize(object obj, IByteBuffer buffer, int order, bool isRequire = true, TarsConvertOptions options = null);
    }
}