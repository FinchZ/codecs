using DotNetty.Buffers;
using System;
using System.Text;

namespace Tars.Net.Codecs
{
    public class CustomTarsStreamConvert : ITarsStreamConvert
    {
        private readonly Func<IByteBuffer, Encoding, object> deserialize;
        private readonly Func<object, Encoding, IByteBuffer> serialize;

        public CustomTarsStreamConvert(Func<IByteBuffer, Encoding, object> deserialize,
            Func<object, Encoding, IByteBuffer> serialize)
        {
            this.deserialize = deserialize;
            this.serialize = serialize;
        }

        public object Deserialize(IByteBuffer buffer, Encoding encoding)
        {
            return deserialize(buffer, encoding);
        }

        public IByteBuffer Serialize(object obj, Encoding encoding)
        {
            return serialize(obj, encoding);
        }
    }
}