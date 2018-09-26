using DotNetty.Buffers;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class BoolTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public BoolTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(true, TarsCodecsVersion.V1, Codec.Tars, 11, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V1, Codec.Tars, 15, 0, TarsStructType.Zero)]
        [InlineData(true, TarsCodecsVersion.V2, Codec.Tars, 41, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V2, Codec.Tars, 115, 0, TarsStructType.Zero)]
        [InlineData(true, TarsCodecsVersion.V3, Codec.Tars, 233, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V3, Codec.Tars, 255, 0, TarsStructType.Zero)]
        public void ShouldEqualExpect(bool obj, short version, Codec codec, int tag, byte byteValue, byte tarsStructType)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            sut.Serialize(obj, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<byte>(buffer, options);
            Assert.Equal(byteValue, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            Assert.Equal(obj, sut.Deserialize<bool>(buffer, options));
        }

        [Theory]
        [InlineData(true, TarsCodecsVersion.V1, Codec.Tars, 11, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V1, Codec.Tars, 15, 0, TarsStructType.Zero)]
        [InlineData(true, TarsCodecsVersion.V2, Codec.Tars, 41, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V2, Codec.Tars, 115, 0, TarsStructType.Zero)]
        [InlineData(true, TarsCodecsVersion.V3, Codec.Tars, 233, 1, TarsStructType.Byte)]
        [InlineData(false, TarsCodecsVersion.V3, Codec.Tars, 255, 0, TarsStructType.Zero)]
        public void WhenNullableShouldEqualExpect(bool obj, short version, Codec codec, int tag, byte byteValue, byte tarsStructType)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            bool? nullable = obj;
            sut.Serialize(nullable, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<byte>(buffer, options);
            Assert.Equal(byteValue, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            Assert.Equal(obj, sut.Deserialize<bool?>(buffer, options));
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 111)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 33)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 115)]
        public void WhenNullShouldBeEmpty(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            bool? nullable = null;
            sut.Serialize(nullable, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }
    }
}