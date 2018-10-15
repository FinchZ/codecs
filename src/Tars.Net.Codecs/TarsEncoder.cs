using DotNetty.Buffers;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsEncoder : IEncoder<IByteBuffer>
    {
        private readonly ITarsConvertRoot convertRoot;

        public TarsEncoder(ITarsConvertRoot convertRoot)
        {
            this.convertRoot = convertRoot;
        }

        public IByteBuffer EncodeRequest(Request req)
        {
            var buffer = Unpooled.Buffer(128);
            convertRoot.Serialize(req, buffer, new TarsConvertOptions());
            return buffer;
        }

        public IByteBuffer EncodeResponse(Response message)
        {
            var buffer = Unpooled.Buffer(128);
            convertRoot.Serialize(message, buffer, new TarsConvertOptions());
            return buffer;
        }
    }
}