using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Tars.Net.Codecs;
using Xunit;

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
        public void ShouldEqualExpect(byte obj, short version, Codec codec, int tag, int length)
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
            Assert.Equal(TarsStructType.Byte, options.TarsType);
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
            sut.Serialize(byte.MinValue, buffer, options);
            Assert.Equal(length, buffer.ReadableBytes);
            var bytes = new byte[length];
            buffer.ReadBytes(bytes);
            Assert.Equal(value, bytes.Last());
            options.Tag = 0;
            buffer = buffer.ResetReaderIndex();
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<byte>(buffer, options);
            Assert.Equal(result, byte.MinValue);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Zero, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11, 0)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133, 2)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255, 255)]
        public void WhenNoValueShouldNoBytes(short version, Codec codec, int tag, byte value)
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
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "ByteTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "ByteTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "ByteTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Float, "ByteTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Double, "ByteTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String1, "ByteTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String4, "ByteTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Map, "ByteTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "ByteTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "ByteTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "ByteTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.ByteArray, "ByteTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "ByteTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "ByteTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "ByteTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Float, "ByteTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Double, "ByteTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String1, "ByteTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String4, "ByteTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Map, "ByteTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "ByteTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "ByteTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "ByteTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.ByteArray, "ByteTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "ByteTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "ByteTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "ByteTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Float, "ByteTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Double, "ByteTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String1, "ByteTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String4, "ByteTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Map, "ByteTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "ByteTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "ByteTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "ByteTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.ByteArray, "ByteTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeZERO_TAGAndByte(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<byte>(buffer, options));
            Assert.Equal(error, ex.Message);
        }
    }
}