using DotNetty.Buffers;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class TarsHeadHandlerTest
    {
        private readonly ITarsHeadHandler sut;

        public TarsHeadHandlerTest()
        {
            sut = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(TarsStructType.Byte, 1)]
        [InlineData(TarsStructType.Short, 1)]
        [InlineData(TarsStructType.Int, 1)]
        [InlineData(TarsStructType.Long, 1)]
        [InlineData(TarsStructType.Float, 1)]
        [InlineData(TarsStructType.Double, 1)]
        [InlineData(TarsStructType.String1, 1)]
        [InlineData(TarsStructType.String4, 1)]
        [InlineData(TarsStructType.Map, 1)]
        [InlineData(TarsStructType.List, 1)]
        [InlineData(TarsStructType.StructBegin, 1)]
        [InlineData(TarsStructType.StructEnd, 1)]
        [InlineData(TarsStructType.Zero, 1)]
        [InlineData(TarsStructType.ByteArray, 1)]
        [InlineData(TarsStructType.Byte, 255)]
        [InlineData(TarsStructType.Short, 255)]
        [InlineData(TarsStructType.Int, 255)]
        [InlineData(TarsStructType.Long, 255)]
        [InlineData(TarsStructType.Float, 255)]
        [InlineData(TarsStructType.Double, 255)]
        [InlineData(TarsStructType.String1, 255)]
        [InlineData(TarsStructType.String4, 255)]
        [InlineData(TarsStructType.Map, 255)]
        [InlineData(TarsStructType.List, 255)]
        [InlineData(TarsStructType.StructBegin, 255)]
        [InlineData(TarsStructType.StructEnd, 255)]
        [InlineData(TarsStructType.Zero, 255)]
        [InlineData(TarsStructType.ByteArray, 255)]
        [InlineData(TarsStructType.Byte, 0)]
        [InlineData(TarsStructType.Short, 0)]
        [InlineData(TarsStructType.Int, 0)]
        [InlineData(TarsStructType.Long, 0)]
        [InlineData(TarsStructType.Float, 0)]
        [InlineData(TarsStructType.Double, 0)]
        [InlineData(TarsStructType.String1, 0)]
        [InlineData(TarsStructType.String4, 0)]
        [InlineData(TarsStructType.Map, 0)]
        [InlineData(TarsStructType.List, 0)]
        [InlineData(TarsStructType.StructBegin, 0)]
        [InlineData(TarsStructType.StructEnd, 0)]
        [InlineData(TarsStructType.Zero, 0)]
        [InlineData(TarsStructType.ByteArray, 0)]
        [InlineData(TarsStructType.Byte, 15)]
        [InlineData(TarsStructType.Short, 15)]
        [InlineData(TarsStructType.Int, 15)]
        [InlineData(TarsStructType.Long, 15)]
        [InlineData(TarsStructType.Float, 15)]
        [InlineData(TarsStructType.Double, 15)]
        [InlineData(TarsStructType.String1, 15)]
        [InlineData(TarsStructType.String4, 15)]
        [InlineData(TarsStructType.Map, 15)]
        [InlineData(TarsStructType.List, 15)]
        [InlineData(TarsStructType.StructBegin, 15)]
        [InlineData(TarsStructType.StructEnd, 15)]
        [InlineData(TarsStructType.Zero, 15)]
        [InlineData(TarsStructType.ByteArray, 15)]
        [InlineData(TarsStructType.Byte, 25)]
        [InlineData(TarsStructType.Short, 25)]
        [InlineData(TarsStructType.Int, 25)]
        [InlineData(TarsStructType.Long, 25)]
        [InlineData(TarsStructType.Float, 25)]
        [InlineData(TarsStructType.Double, 25)]
        [InlineData(TarsStructType.String1, 25)]
        [InlineData(TarsStructType.String4, 25)]
        [InlineData(TarsStructType.Map, 25)]
        [InlineData(TarsStructType.List, 25)]
        [InlineData(TarsStructType.StructBegin, 25)]
        [InlineData(TarsStructType.StructEnd, 25)]
        [InlineData(TarsStructType.Zero, 25)]
        [InlineData(TarsStructType.ByteArray, 25)]
        public void ShouldEqualExpect(byte type, int tag)
        {
            var buffer = Unpooled.Buffer(2);
            sut.WriteHead(buffer, type, tag);
            var options = new TarsConvertOptions();
            sut.ReadHead(buffer, options);
            Assert.Equal(type, options.TarsType);
            Assert.Equal(tag, options.Tag);
        }

        [Theory]
        [InlineData(TarsStructType.Byte, 256)]
        [InlineData(TarsStructType.Short, 256)]
        [InlineData(TarsStructType.Int, 256)]
        [InlineData(TarsStructType.Long, 256)]
        [InlineData(TarsStructType.Float, 256)]
        [InlineData(TarsStructType.Double, 256)]
        [InlineData(TarsStructType.String1, 256)]
        [InlineData(TarsStructType.String4, 256)]
        [InlineData(TarsStructType.Map, 256)]
        [InlineData(TarsStructType.List, 256)]
        [InlineData(TarsStructType.StructBegin, 256)]
        [InlineData(TarsStructType.StructEnd, 256)]
        [InlineData(TarsStructType.Zero, 256)]
        [InlineData(TarsStructType.ByteArray, 256)]
        public void WhenTagBigThan255ShouldEx(byte type, int tag)
        {
            var buffer = Unpooled.Buffer(2);
            var ex = Assert.Throws<TarsEncodeException>(() => sut.WriteHead(buffer, type, tag));
            Assert.Equal("tag is too large: " + tag, ex.Message);
        }
    }
}