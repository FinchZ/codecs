using DotNetty.Buffers;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class TaskTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public TaskTarsConvertTest()
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
        [InlineData(int.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(999999, TarsCodecsVersion.V1, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(int.MinValue, TarsCodecsVersion.V1, Codec.Tars, 33, TarsStructType.Int)]
        [InlineData(long.MaxValue, TarsCodecsVersion.V1, Codec.Tars, 233, TarsStructType.Long)]
        [InlineData(9999999999, TarsCodecsVersion.V1, Codec.Tars, 123, TarsStructType.Long)]
        [InlineData(long.MinValue, TarsCodecsVersion.V1, Codec.Tars, 110, TarsStructType.Long)]
        [InlineData(0, TarsCodecsVersion.V2, Codec.Tars, 231, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V2, Codec.Tars, 175, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V2, Codec.Tars, 44, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 88, TarsStructType.Short)]
        [InlineData(int.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(999999, TarsCodecsVersion.V2, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(int.MinValue, TarsCodecsVersion.V2, Codec.Tars, 6, TarsStructType.Int)]
        [InlineData(long.MaxValue, TarsCodecsVersion.V2, Codec.Tars, 3, TarsStructType.Long)]
        [InlineData(9999999999, TarsCodecsVersion.V2, Codec.Tars, 7, TarsStructType.Long)]
        [InlineData(long.MinValue, TarsCodecsVersion.V2, Codec.Tars, 2, TarsStructType.Long)]
        [InlineData(0, TarsCodecsVersion.V3, Codec.Tars, 228, TarsStructType.Zero)]
        [InlineData(255, TarsCodecsVersion.V3, Codec.Tars, 177, TarsStructType.Byte)]
        [InlineData(256, TarsCodecsVersion.V3, Codec.Tars, 15, TarsStructType.Short)]
        [InlineData(short.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 16, TarsStructType.Short)]
        [InlineData(int.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(999999, TarsCodecsVersion.V3, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(int.MinValue, TarsCodecsVersion.V3, Codec.Tars, 0, TarsStructType.Int)]
        [InlineData(long.MaxValue, TarsCodecsVersion.V3, Codec.Tars, 24, TarsStructType.Long)]
        [InlineData(9999999999, TarsCodecsVersion.V3, Codec.Tars, 10, TarsStructType.Long)]
        [InlineData(long.MinValue, TarsCodecsVersion.V3, Codec.Tars, 20, TarsStructType.Long)]
        public void ShouldEqualExpect(long obj, short version, Codec codec, int tag, byte tarsStructType)
        {
            var buffer = Unpooled.Buffer(1);
            var options = new TarsConvertOptions()
            {
                Codec = codec,
                Version = version,
                Tag = tag
            };
            sut.Serialize(Task.FromResult(obj), buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<Task<long>>(buffer, options);
            Assert.Equal(obj, result.Result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }
    }
}