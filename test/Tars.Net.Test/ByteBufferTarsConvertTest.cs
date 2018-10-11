using DotNetty.Buffers;
using System.Text;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class ByteBufferTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public ByteBufferTarsConvertTest()
        {
            var test = new TestTarsConvert();
            sut = test.ConvertRoot;
            headHandler = test.HeadHandler;
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 131)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 23)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 215)]
        public void WhenNullShouldBeNoBytes(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            sut.Serialize<IByteBuffer>(null, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 131)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 23)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 215)]
        public void WhenEmptyShouldBeEmpty(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            sut.Serialize(Unpooled.Empty, buffer, options);
            Assert.True(options.HasValue);
            Assert.True(buffer.ReadableBytes > 0);
            options.Tag = 0;
            headHandler.ReadHead(buffer, options);
            Assert.Equal(TarsStructType.ByteArray, options.TarsType);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(Unpooled.Empty, sut.Deserialize<IByteBuffer>(buffer, options));
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 131, "test")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 23, "testafaa")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 215, "test阿凡达的")]
        public void WhenHasBytesShouldNotEmpty(short version, Codec codec, int tag, string str)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            sut.Serialize(Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(str)), buffer, options);
            Assert.True(options.HasValue);
            Assert.True(buffer.ReadableBytes > 0);
            options.Tag = 0;
            headHandler.ReadHead(buffer, options);
            Assert.Equal(TarsStructType.ByteArray, options.TarsType);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(str, Encoding.UTF8.GetString(sut.Deserialize<IByteBuffer>(buffer, options).Slice().Array));
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Byte, "ByteBufferTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "ByteBufferTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "ByteBufferTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "ByteBufferTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Float, "ByteBufferTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Double, "ByteBufferTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String1, "ByteBufferTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String4, "ByteBufferTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Map, "ByteBufferTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "ByteBufferTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "ByteBufferTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "ByteBufferTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Zero, "ByteBufferTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Byte, "ByteBufferTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "ByteBufferTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "ByteBufferTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "ByteBufferTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Float, "ByteBufferTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Double, "ByteBufferTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String1, "ByteBufferTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String4, "ByteBufferTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Map, "ByteBufferTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "ByteBufferTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "ByteBufferTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "ByteBufferTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Zero, "ByteBufferTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Byte, "ByteBufferTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "ByteBufferTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "ByteBufferTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "ByteBufferTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Float, "ByteBufferTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Double, "ByteBufferTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String1, "ByteBufferTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String4, "ByteBufferTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Map, "ByteBufferTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "ByteBufferTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "ByteBufferTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "ByteBufferTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Zero, "ByteBufferTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeByteArray(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<IByteBuffer>(buffer, options));
            Assert.Equal(error, ex.Message);
        }
    }
}