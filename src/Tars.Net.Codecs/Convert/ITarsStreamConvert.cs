using System.Text;
using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public interface ITarsStreamConvert
    {
        object Deserialize(IByteBuffer buffer, Encoding encoding);

        IByteBuffer Serialize(object obj, Encoding encoding);
    }
}