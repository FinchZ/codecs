using DotNetty.Buffers;
using System.Collections.Generic;
using Tars.Net.Codecs;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.Test
{
    public class ResponseTarsConvertTest
    {
        private readonly TarsDecoder decoder;
        private readonly TarsEncoder encoder;

        public ResponseTarsConvertTest()
        {
            decoder = new TarsDecoder(TestTarsConvert.ConvertRoot);
            encoder = new TarsEncoder(TestTarsConvert.ConvertRoot);
        }

        [Theory]
        //[InlineData(TarsCodecsVersion.V1, Codec.Tars)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars)]
        public int ResponseShouldEqualExpect(short version, Codec codec)
        {
            Unpooled.Buffer(1);
            var resp = new Response()
            {
                Version = version,
                Codec = codec,
                PacketType = 3,
                MessageType = 4,
                RequestId = 66,
                Timeout = 77,
                ServantName = "aa.aa",
                FuncName = "go",
                Context = new Dictionary<string, string>() { { "a", "b" } },
                Status = new Dictionary<string, string>() { { "a1", "b2" } },
                ReturnParameters = new object[2] { version, codec },
                ReturnValue = 55,
                ResultStatusCode = RpcStatusCode.ServerSuccess,
                ResultDesc = "test"
            };
            var method = GetType().GetMethod("ResponseShouldEqualExpect");
            resp.ReturnParameterTypes = method.GetParameters();
            resp.ReturnValueType = method.ReturnParameter;
            TestTarsConvert.FindRpcMethodFunc = (servantName, funcName) =>
            {
                return (method, true, method.GetParameters(), codec, version, method.DeclaringType);
            };
            var buffer = encoder.EncodeResponse(resp);
            var result = decoder.DecodeResponse(buffer);

            Assert.Equal(resp.Version, result.Version);
            Assert.Equal(resp.PacketType, result.PacketType);
            Assert.Equal(resp.MessageType, result.MessageType);
            Assert.Equal(resp.RequestId, result.RequestId);
            if (version != 1)
            {
                Assert.Equal(resp.ServantName, result.ServantName);
                Assert.Equal(resp.FuncName, result.FuncName);
            }
            Assert.Equal(resp.Context["a"], result.Context["a"]);
            Assert.Single(resp.Context);
            Assert.Equal(resp.Status["a1"], result.Status["a1"]);
            Assert.Single(resp.Status);
            Assert.Equal(resp.ReturnParameters[0], result.ReturnParameters[0]);
            Assert.Equal(resp.ReturnParameters[1], result.ReturnParameters[1]);
            Assert.Equal(resp.ReturnValue, result.ReturnValue);
            Assert.Equal(resp.ResultStatusCode, result.ResultStatusCode);
            return 0;
        }
    }
}