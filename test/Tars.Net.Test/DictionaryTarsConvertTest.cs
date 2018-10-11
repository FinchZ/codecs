using DotNetty.Buffers;
using System.Collections.Generic;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class DictionaryTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public DictionaryTarsConvertTest()
        {
            var test = new TestTarsConvert();
            sut = test.ConvertRoot;
            headHandler = test.HeadHandler;
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 15)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 41)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 233)]
        public void ShouldEqualExpect(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            var dict = new Dictionary<string, string>
            {
                { "str", "ok" }
            } as IDictionary<string, string>;
            sut.Serialize(dict, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<IDictionary<string, string>>(buffer, options);
            Assert.Single(result);
            Assert.Equal("ok", result["str"]);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Map, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 15)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 41)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 233)]
        public void DictShouldEqualExpect(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            var dict = new Dictionary<string, string>
            {
                { "str", "ok" }
            };
            sut.Serialize(dict, buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<Dictionary<string, string>>(buffer, options);
            Assert.Single(result);
            Assert.Equal("ok", result["str"]);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Map, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 15)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 41)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 233)]
        public void SameKeyShouldBeSingle(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            headHandler.Reserve(buffer, 8);
            headHandler.WriteHead(buffer, TarsStructType.Map, options.Tag);
            buffer.WriteInt(2);
            for (int i = 0; i < 2; i++)
            {
                options.Tag = 0;
                sut.Serialize("str", buffer, options);
                options.Tag = 1;
                sut.Serialize("ok" + i.ToString(), buffer, options);
            }
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<IDictionary<string, string>>(buffer, options);
            Assert.Single(result);
            Assert.Equal("ok1", result["str"]);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(TarsStructType.Map, options.TarsType);
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
            IDictionary<string, string> nullable = null;
            sut.Serialize(nullable, buffer, options);
            Assert.True(options.HasValue);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Byte, "DictionaryTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "DictionaryTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "DictionaryTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "DictionaryTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Float, "DictionaryTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Double, "DictionaryTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String1, "DictionaryTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.String4, "DictionaryTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "DictionaryTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "DictionaryTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "DictionaryTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Zero, "DictionaryTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.ByteArray, "DictionaryTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Byte, "DictionaryTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "DictionaryTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "DictionaryTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "DictionaryTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Float, "DictionaryTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Double, "DictionaryTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String1, "DictionaryTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.String4, "DictionaryTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "DictionaryTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "DictionaryTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "DictionaryTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Zero, "DictionaryTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.ByteArray, "DictionaryTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Byte, "DictionaryTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "DictionaryTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "DictionaryTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "DictionaryTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Float, "DictionaryTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Double, "DictionaryTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String1, "DictionaryTarsConvert can not deserialize TarsType:6,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.String4, "DictionaryTarsConvert can not deserialize TarsType:7,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "DictionaryTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "DictionaryTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "DictionaryTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Zero, "DictionaryTarsConvert can not deserialize TarsType:12,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.ByteArray, "DictionaryTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeMap(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<IDictionary<string, string>>(buffer, options));
            Assert.Equal(error, ex.Message);
        }
    }
}