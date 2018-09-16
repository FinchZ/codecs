using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public interface ITarsConvert
    {
        IByteBuffer Serialize(object obj, TarsConvertOptions options = null);

        object Deserialize(IByteBuffer buffer, Type type, TarsConvertOptions options = null);
    }
}