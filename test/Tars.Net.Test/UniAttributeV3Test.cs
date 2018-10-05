using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class UniAttributeV3Test
    {
        private readonly ITarsHeadHandler headHandler;

        public UniAttributeV3Test()
        {
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(Codec.Tars, 231)]
        [InlineData(Codec.Tars, 33)]
        public void UniAttributeTest(Codec codec, int tag)
        {
            var sut = new UniAttributeV3(TestTarsConvert.ConvertRoot, TestTarsConvert.HeadHandler)
            {
                Temp = new Dictionary<string, IByteBuffer>(3)
            };
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = TarsCodecsVersion.V3,
                Tag = tag,
            };
            sut.Put(string.Empty, 1, typeof(int), options);
            sut.Put("23", "66", typeof(string), options);
            sut.Put("23", "23", typeof(string), options);
            sut.Put("dd", 0d, typeof(double), options);
            sut.Put("ddd", 0d, typeof(double), options);
            sut.Put("ddd", null, typeof(double), options);
            var buf = Unpooled.Buffer(128);
            sut.Serialize(buf, options);
            sut.Temp = null;
            headHandler.ReadHead(buf, options);
            sut.Deserialize(buf, options);
            Assert.Equal(3, sut.Temp.Count);
            var buffer = sut.Temp[string.Empty];
            TestTarsConvert.HeadHandler.ReadHead(buffer, options);
            Assert.Equal(1, TestTarsConvert.ConvertRoot.Deserialize<int>(buffer, options));
            buffer = sut.Temp["23"];
            TestTarsConvert.HeadHandler.ReadHead(buffer, options);
            Assert.Equal("23", TestTarsConvert.ConvertRoot.Deserialize<string>(buffer, options));
            buffer = sut.Temp["dd"];
            TestTarsConvert.HeadHandler.ReadHead(buffer, options);
            Assert.Equal(0d, TestTarsConvert.ConvertRoot.Deserialize<double>(buffer, options));
        }

        [Theory]
        [InlineData(Codec.Tars, 231)]
        public void UniAttributeArgumentExceptionTest(Codec codec, int tag)
        {
            var sut = new UniAttributeV3(TestTarsConvert.ConvertRoot, TestTarsConvert.HeadHandler)
            {
                Temp = new Dictionary<string, IByteBuffer>(3)
            };
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = TarsCodecsVersion.V3,
                Tag = tag,
            };
            var ex = Assert.Throws<ArgumentException>(() => sut.Put(null, 1, typeof(int), options));
            Assert.Equal("put key can not be null", ex.Message);
        }
    }
}