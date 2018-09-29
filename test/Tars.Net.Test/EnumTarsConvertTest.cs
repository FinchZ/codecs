using DotNetty.Buffers;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class EnumTarsConvertTest
    {
        public enum IntEnum
        {
            A,
            B = short.MaxValue,
            C = int.MaxValue
        }

        public enum ByteEnum : byte
        {
            A,
            B = 3,
            C = 5
        }

        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public EnumTarsConvertTest()
        {
            sut = TestTarsConvert.ConvertRoot;
            headHandler = TestTarsConvert.HeadHandler;
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 231)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 33)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 115)]
        public void WhenIntEnumShouldEqualExpect(short version, Codec codec, int tag)
        {
            var enums = new IntEnum[] { IntEnum.A, IntEnum.B, IntEnum.C };
            var types = new byte[] { TarsStructType.Zero, TarsStructType.Short, TarsStructType.Int };
            for (int i = 0; i < enums.Length; i++)
            {
                var item = enums[i];
                var buffer = Unpooled.Buffer(1);
                var options = new TarsConvertOptions()
                {
                    Codec = codec,
                    Version = version,
                    Tag = tag,
                };
                sut.Serialize(item, buffer, options);
                Assert.True(options.HasValue);
                Assert.True(buffer.ReadableBytes > 0);
                options.Tag = 0;
                headHandler.ReadHead(buffer, options);
                Assert.Equal(types[i], options.TarsType);
                Assert.Equal(tag, options.Tag);
                Assert.Equal(item, sut.Deserialize<IntEnum>(buffer, options));
            }
        }

        [Theory]
        [InlineData(TarsCodecsVersion.V1, Codec.Tars, 231)]
        [InlineData(TarsCodecsVersion.V2, Codec.Tars, 33)]
        [InlineData(TarsCodecsVersion.V3, Codec.Tars, 115)]
        public void WhenByteEnumShouldEqualExpect(short version, Codec codec, int tag)
        {
            var enums = new ByteEnum[] { ByteEnum.A, ByteEnum.B, ByteEnum.C };
            var types = new byte[] { TarsStructType.Zero, TarsStructType.Byte, TarsStructType.Byte };
            for (int i = 0; i < enums.Length; i++)
            {
                var item = enums[i];
                var buffer = Unpooled.Buffer(1);
                var options = new TarsConvertOptions()
                {
                    Codec = codec,
                    Version = version,
                    Tag = tag,
                };
                sut.Serialize(item, buffer, options);
                Assert.True(options.HasValue);
                Assert.True(buffer.ReadableBytes > 0);
                options.Tag = 0;
                headHandler.ReadHead(buffer, options);
                Assert.Equal(types[i], options.TarsType);
                Assert.Equal(tag, options.Tag);
                Assert.Equal(item, sut.Deserialize<ByteEnum>(buffer, options));
            }
        }
    }
}