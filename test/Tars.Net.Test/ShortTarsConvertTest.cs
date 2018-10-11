using DotNetty.Buffers;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class ShortTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public ShortTarsConvertTest()
        {
            var test = new TestTarsConvert();
            sut = test.ConvertRoot;
            headHandler = test.HeadHandler;
        }

        [Theory]
        [InlineData(0, TarsCodecsVersion.V1, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V1, Codec.Tars, 115, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V1, Codec.Tars, 10, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 0, TarsStructType.Short)]
        [InlineData(0, TarsCodecsVersion.V2, Codec.Tars, 231, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V2, Codec.Tars, 175, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V2, Codec.Tars, 44, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 88, TarsStructType.Short)]
        [InlineData(0, TarsCodecsVersion.V3, Codec.Tars, 228, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V3, Codec.Tars, 177, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V3, Codec.Tars, 15, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 16, TarsStructType.Short)]
        public void ShouldEqualExpect(short obj, short version, Codec codec, int tag, byte tarsStructType)
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
            var result = sut.Deserialize<short>(buffer, options);
            Assert.Equal(obj, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 131)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 23)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 215)]
        public void WhenNullShouldBeEmpty(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            short? nullable = null;
            sut.Serialize(nullable, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(0, TarsCodecsVersion.V1, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V1, Codec.Tars, 115, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V1, Codec.Tars, 10, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 0, TarsStructType.Short)]
        [InlineData(0, TarsCodecsVersion.V2, Codec.Tars, 231, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V2, Codec.Tars, 175, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V2, Codec.Tars, 44, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 88, TarsStructType.Short)]
        [InlineData(0, TarsCodecsVersion.V3, Codec.Tars, 228, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V3, Codec.Tars, 177, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V3, Codec.Tars, 15, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 16, TarsStructType.Short)]
        public void WhenNullableShouldEqualExpect(short obj, short version, Codec codec, int tag, byte tarsStructType)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            short? nullable = obj;
            sut.Serialize(nullable, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<short?>(buffer, options);
            Assert.Equal(obj, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }
    }
}