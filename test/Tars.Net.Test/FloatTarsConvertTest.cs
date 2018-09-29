using DotNetty.Buffers;
using System.Linq;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class FloatTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public FloatTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(1.523f, TarsCodecsVersion.V1, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V1, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V1, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V1, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V1, Codec.Tars, 41, 6)]
        [InlineData(1.523f, TarsCodecsVersion.V2, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V2, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V2, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V2, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V2, Codec.Tars, 41, 6)]
        [InlineData(1.523f, TarsCodecsVersion.V3, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V3, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V3, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V3, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V3, Codec.Tars, 41, 6)]
        public void ShouldEqualExpect(float obj, short version, Codec codec, int tag, int length)
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
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<float>(buffer, options);
            Assert.Equal(result, obj);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Float, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11, 188)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133, 133)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255, 255)]
        public void WhenZeroShouldBeZERO_TAG(short version, Codec codec, int tag, byte value)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            sut.Serialize(0f, buffer, options);
            var bytes = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(bytes);
            Assert.Equal(value, bytes.Last());
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<float>(buffer, options);
            Assert.Equal(0, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Zero, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11, 0)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133, 2)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255, 255)]
        public void WhenNoValueShouldNoBytes(short version, Codec codec, int tag, float value)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
                HasValue = false
            };
            sut.Serialize(value, buffer, options);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Byte, "FloatTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "FloatTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "FloatTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "FloatTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Double, "FloatTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String1, "FloatTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String4, "FloatTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Map, "FloatTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "FloatTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "FloatTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "FloatTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.ByteArray, "FloatTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Byte, "FloatTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "FloatTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "FloatTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "FloatTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Double, "FloatTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String1, "FloatTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String4, "FloatTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Map, "FloatTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "FloatTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "FloatTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "FloatTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.ByteArray, "FloatTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Byte, "FloatTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "FloatTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "FloatTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "FloatTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Double, "FloatTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String1, "FloatTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String4, "FloatTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Map, "FloatTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "FloatTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "FloatTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "FloatTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.ByteArray, "FloatTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeZERO_TAGAndFloat(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<float>(buffer, options));
            Assert.Equal(error, ex.Message);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255)]
        public void WhenNullShouldBeEmpty(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag,
            };
            float? nullable = null;
            sut.Serialize(nullable, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(1.523f, TarsCodecsVersion.V1, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V1, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V1, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V1, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V1, Codec.Tars, 41, 6)]
        [InlineData(1.523f, TarsCodecsVersion.V2, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V2, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V2, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V2, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V2, Codec.Tars, 41, 6)]
        [InlineData(1.523f, TarsCodecsVersion.V3, Codec.Tars, 11, 5)]
        [InlineData(1.123f, TarsCodecsVersion.V3, Codec.Tars, 15, 6)]
        [InlineData(23.323f, TarsCodecsVersion.V3, Codec.Tars, 255, 6)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 21, 6)]
        [InlineData(666.44f, TarsCodecsVersion.V3, Codec.Tars, 1, 5)]
        [InlineData(float.MinValue, TarsCodecsVersion.V3, Codec.Tars, 41, 6)]
        public void WhenNullableShouldEqualExpect(float obj, short version, Codec codec, int tag, int length)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            float? nullable = obj;
            sut.Serialize(nullable, buffer, options);
            Assert.Equal(length, buffer.ReadableBytes);
            var bytes = new byte[length];
            buffer.ReadBytes(bytes);
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<float?>(buffer, options);
            Assert.Equal(result, obj);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Float, options.TarsType);
        }
    }
}