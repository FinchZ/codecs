using DotNetty.Buffers;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.Test
{
    public class RequestTarsConvertTest
    {
        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars)]
        public void ShouldEqualExpect(short version, Codec codec)
        {
            Unpooled.Buffer(1);
            var request = new Request()
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
                Parameters = new object[2] { version, codec }
            };
            var method = GetType().GetMethod("ShouldEqualExpect");
            request.ParameterTypes = method.GetParameters();
            var test = new TestTarsConvert();
            var decoder = new TarsDecoder(test.ConvertRoot);
            var encoder = new TarsEncoder(test.ConvertRoot);
            test.FindRpcMethodFunc = (servantName, funcName) =>
            {
                return (method, true, new ParameterInfo[0], codec, version, method.DeclaringType);
            };
            var buffer = encoder.EncodeRequest(request);
            var result = decoder.DecodeRequest(buffer);

            Assert.Equal(request.Version, result.Version);
            Assert.Equal(request.PacketType, result.PacketType);
            Assert.Equal(request.MessageType, result.MessageType);
            Assert.Equal(request.RequestId, result.RequestId);
            Assert.Equal(request.Timeout, result.Timeout);
            Assert.Equal(request.ServantName, result.ServantName);
            Assert.Equal(request.FuncName, result.FuncName);
            Assert.Equal(request.Context["a"], result.Context["a"]);
            Assert.Single(request.Context);
            Assert.Equal(request.Status["a1"], result.Status["a1"]);
            Assert.Single(request.Status);
            Assert.Equal(request.Parameters[0], result.Parameters[0]);
            Assert.Equal(request.Parameters[1], result.Parameters[1]);
            Assert.Equal(request.Parameters.Length, result.Parameters.Length);
        }
    }
}