using DotNetty.Buffers;
using System.Linq;
using Tars.Net.Codecs;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Test
{
    public class ByteTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsConvert<byte> convert;
        private readonly ITarsHeadHandler headHandler;

        public ByteTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            convert = TestTarsConvert.Provider.GetRequiredService<ITarsConvert<byte>>();
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData((byte)1, TarsCodecsVersion.V1, Codec.Tars, 11, 2)]
        [InlineData((byte)1, TarsCodecsVersion.V1, Codec.Tars, 15, 3)]
        [InlineData((byte)23, TarsCodecsVersion.V1, Codec.Tars, 255, 3)]
        [InlineData(byte.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 1, 2)]
        [InlineData((byte)1, TarsCodecsVersion.V2, Codec.Tars, 11, 2)]
        [InlineData((byte)1, TarsCodecsVersion.V2, Codec.Tars, 15, 3)]
        [InlineData((byte)23, TarsCodecsVersion.V2, Codec.Tars, 255, 3)]
        [InlineData(byte.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 1, 2)]
        [InlineData((byte)1, TarsCodecsVersion.V3, Codec.Tars, 11, 2)]
        [InlineData((byte)1, TarsCodecsVersion.V3, Codec.Tars, 15, 3)]
        [InlineData((byte)23, TarsCodecsVersion.V3, Codec.Tars, 255, 3)]
        [InlineData(byte.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 1, 2)]
        public void ByteConvertShouldEqual(byte obj, short version, Codec codec, int tag, int length)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            sut.Serialize(obj, buffer, options);
            Assert.Equal(length, buffer.ReadableBytes);
            var bytes = new byte[length];
            buffer.ReadBytes(bytes);
            Assert.Equal(obj, bytes.Last());
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<byte>(buffer, options);
            Assert.Equal(result, obj);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.BYTE, options.TarsType);
        }
    }
}