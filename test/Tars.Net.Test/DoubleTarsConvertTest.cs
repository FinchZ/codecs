using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class DoubleTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public DoubleTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(0d, TarsCodecsVersion.V1, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255.23d, TarsCodecsVersion.V1, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V1, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V1, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(232323.231313d, TarsCodecsVersion.V1, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V1, Codec.Tars, 64, TarsStructType.Double)]
        [InlineData(0d, TarsCodecsVersion.V2, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255.23d, TarsCodecsVersion.V2, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V2, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V2, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(1666232323.231313, TarsCodecsVersion.V2, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V2, Codec.Tars, 64, TarsStructType.Double)]
        [InlineData(0d, TarsCodecsVersion.V3, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(234.23d, TarsCodecsVersion.V3, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V3, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V3, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(232323.23d, TarsCodecsVersion.V3, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V3, Codec.Tars, 64, TarsStructType.Double)]
        public void ShouldEqualExpect(double obj, short version, Codec codec, int tag, byte tarsStructType)
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
            var result = sut.Deserialize<double>(buffer, options);
            Assert.Equal(obj, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11, 1, 188)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133, 2, 133)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255, 2, 255)]
        public void WhenZeroShouldBeZERO_TAG(short version, Codec codec, int tag, int length, byte value)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            sut.Serialize(0d, buffer, options);
            Assert.Equal(length, buffer.ReadableBytes);
            var bytes = new byte[length];
            buffer.ReadBytes(bytes);
            Assert.Equal(value, bytes.Last());
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<double>(buffer, options);
            Assert.Equal(result, byte.MinValue);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Zero, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11, 0)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133, 2)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255, 255)]
        public void WhenNoValueShouldNoBytes(short version, Codec codec, int tag, double value)
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
            double? nullable = null;
            sut.Serialize(nullable, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(0d, TarsCodecsVersion.V1, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255.23d, TarsCodecsVersion.V1, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V1, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V1, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(232323.231313d, TarsCodecsVersion.V1, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V1, Codec.Tars, 64, TarsStructType.Double)]
        [InlineData(0d, TarsCodecsVersion.V2, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(255.23d, TarsCodecsVersion.V2, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V2, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V2, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(1666232323.231313, TarsCodecsVersion.V2, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V2, Codec.Tars, 64, TarsStructType.Double)]
        [InlineData(0d, TarsCodecsVersion.V3, Codec.Tars, 221, TarsStructType.Zero)]
        [InlineData(234.23d, TarsCodecsVersion.V3, Codec.Tars, 115, TarsStructType.Double)]
        [InlineData(256.1231323d, TarsCodecsVersion.V3, Codec.Tars, 10, TarsStructType.Double)]
        [InlineData(float.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 213, TarsStructType.Double)]
        [InlineData(float.MinValue, TarsCodecsVersion.V3, Codec.Tars, 42, TarsStructType.Double)]
        [InlineData(double.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 33, TarsStructType.Double)]
        [InlineData(232323.23d, TarsCodecsVersion.V3, Codec.Tars, 44, TarsStructType.Double)]
        [InlineData(double.MinValue, TarsCodecsVersion.V3, Codec.Tars, 64, TarsStructType.Double)]
        public void WhenNullableShouldEqualExpect(double obj, short version, Codec codec, int tag, byte tarsStructType)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            double? nullable = obj;
            sut.Serialize(nullable, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<double?>(buffer, options);
            Assert.Equal(obj, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Byte, "DoubleTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "DoubleTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "DoubleTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "DoubleTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Float, "DoubleTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String1, "DoubleTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String4, "DoubleTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Map, "DoubleTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "DoubleTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "DoubleTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "DoubleTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.ByteArray, "DoubleTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Byte, "DoubleTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "DoubleTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "DoubleTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "DoubleTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Float, "DoubleTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String1, "DoubleTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String4, "DoubleTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Map, "DoubleTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "DoubleTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "DoubleTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "DoubleTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.ByteArray, "DoubleTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Byte, "DoubleTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "DoubleTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "DoubleTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "DoubleTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Float, "DoubleTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String1, "DoubleTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String4, "DoubleTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Map, "DoubleTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "DoubleTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "DoubleTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "DoubleTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.ByteArray, "DoubleTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeZERO_TAGAndFloat(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<double>(buffer, options));
            Assert.Equal(error, ex.Message);
        }
    }
}
