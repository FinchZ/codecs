using DotNetty.Buffers;
using System;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsDecoder : IDecoder<IByteBuffer>
    {
        private readonly ITarsConvertRoot convertRoot;

        public TarsDecoder(ITarsConvertRoot convertRoot)
        {
            this.convertRoot = convertRoot;
        }

        public Request DecodeRequest(IByteBuffer input)
        {
            try
            {
                return convertRoot.Deserialize<Request>(input, new TarsConvertOptions());
            }
            catch (Exception ex)
            {
                throw new TarsException(RpcStatusCode.ServerDecodeErr, ex);
            }
        }

        public Response DecodeResponse(IByteBuffer input)
        {
            try
            {
                return convertRoot.Deserialize<Response>(input, new TarsConvertOptions());
            }
            catch (Exception ex)
            {
                throw new TarsException(RpcStatusCode.ServerDecodeErr, ex);
            }
        }
    }
}