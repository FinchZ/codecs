using DotNetty.Buffers;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class StringTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public StringTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData("", TarsCodecsVersion.V1, Codec.Tars, 221, TarsStructType.String1)]
        [InlineData("1331adaagfafdadadad", TarsCodecsVersion.V1, Codec.Tars, 21, TarsStructType.String1)]
        [InlineData("中安啊大大", TarsCodecsVersion.V1, Codec.Tars, 121, TarsStructType.String1)]
        [InlineData("中安啊大大啊的法律和咖啡垃圾货垃圾货垃圾货垃圾货大家啊刚好碰上广告歌发票宫颈癌宫颈癌花椒粉辣椒粉破案高炮gap的价格胖啊宫颈癌奥普台风派来喷去泰国脾气大书法家法律纠纷那句话了啊哈哈农行卡然后就拿江湖各派工作两年半", TarsCodecsVersion.V1, Codec.Tars, 151, TarsStructType.String4)]
        [InlineData("", TarsCodecsVersion.V2, Codec.Tars, 221, TarsStructType.String1)]
        [InlineData("1331adaagfafdadadad", TarsCodecsVersion.V2, Codec.Tars, 21, TarsStructType.String1)]
        [InlineData("中安啊大大", TarsCodecsVersion.V2, Codec.Tars, 121, TarsStructType.String1)]
        [InlineData("中安啊大大啊的法律和咖啡垃圾货垃圾货垃圾货垃圾货大家啊刚好碰上广告歌发票宫颈癌宫颈癌花椒粉辣椒粉破案高炮gap的价格胖啊宫颈癌奥普台风派来喷去泰国脾气大书法家法律纠纷那句话了啊哈哈农行卡然后就拿江湖各派工作两年半", TarsCodecsVersion.V2, Codec.Tars, 151, TarsStructType.String4)]
        [InlineData("", TarsCodecsVersion.V3, Codec.Tars, 221, TarsStructType.String1)]
        [InlineData("1331adaagfafdadadad", TarsCodecsVersion.V3, Codec.Tars, 21, TarsStructType.String1)]
        [InlineData("中安啊大大", TarsCodecsVersion.V3, Codec.Tars, 121, TarsStructType.String1)]
        [InlineData("中安啊大大啊的法律和咖啡垃圾货垃圾货垃圾货垃圾货大家啊刚好碰上广告歌发票宫颈癌宫颈癌花椒粉辣椒粉破案高炮gap的价格胖啊宫颈癌奥普台风派来喷去泰国脾气大书法家法律纠纷那句话了啊哈哈农行卡然后就拿江湖各派工作两年半", TarsCodecsVersion.V3, Codec.Tars, 151, TarsStructType.String4)]
        public void ShouldEqualExpect(string obj, short version, Codec codec, int tag, byte tarsStructType)
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
            var result = sut.Deserialize<string>(buffer, options);
            Assert.Equal(obj, result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 11)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 133)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 255)]
        public void WhenNoValueShouldNoBytes(short version, Codec codec, int tag)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            string str = null;
            sut.Serialize(str, buffer, options);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Byte, "StringTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Short, "StringTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Int, "StringTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Long, "StringTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Float, "StringTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Double, "StringTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.Map, "StringTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.List, "StringTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructBegin, "StringTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.StructEnd, "StringTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, TarsStructType.ByteArray, "StringTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:1,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Byte, "StringTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Short, "StringTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Int, "StringTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Long, "StringTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Float, "StringTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Double, "StringTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.Map, "StringTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.List, "StringTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructBegin, "StringTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.StructEnd, "StringTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, TarsStructType.ByteArray, "StringTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:2,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Byte, "StringTarsConvert can not deserialize TarsType:0,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Short, "StringTarsConvert can not deserialize TarsType:1,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Int, "StringTarsConvert can not deserialize TarsType:2,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Long, "StringTarsConvert can not deserialize TarsType:3,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Float, "StringTarsConvert can not deserialize TarsType:4,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Double, "StringTarsConvert can not deserialize TarsType:5,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.Map, "StringTarsConvert can not deserialize TarsType:8,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.List, "StringTarsConvert can not deserialize TarsType:9,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructBegin, "StringTarsConvert can not deserialize TarsType:10,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.StructEnd, "StringTarsConvert can not deserialize TarsType:11,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, TarsStructType.ByteArray, "StringTarsConvert can not deserialize TarsType:13,Codec:Tars,Version:3,Tag:1,Encoding:System.Text.UTF8Encoding+UTF8EncodingSealed")]
        public void ShouldOnlyCanDeserializeString(short version, Codec codec, byte tarsStructType, string error)
        {
            var buffer = Unpooled.WrappedBuffer(new byte[] { 255 });
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                TarsType = tarsStructType
            };
            var ex = Assert.Throws<TarsDecodeException>(() => sut.Deserialize<string>(buffer, options));
            Assert.Equal(error, ex.Message);
        }
    }
}