using DotNetty.Buffers;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.Test
{
    public class ValueTaskTarsConvertTest
    {
        private readonly ITarsConvertRoot sut;
        private readonly ITarsHeadHandler headHandler;

        public ValueTaskTarsConvertTest()
        {
            var test = new TestTarsConvert();
            sut = test.ConvertRoot;
            headHandler = test.HeadHandler;
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
            sut.Serialize(new ValueTask<double>(obj), buffer, options);
            headHandler.ReadHead(buffer, options);
            var result = sut.Deserialize<ValueTask<double>>(buffer, options);
            Assert.Equal(obj, result.Result);
            Assert.Equal(tag, options.Tag);
            Assert.Equal(tarsStructType, options.TarsType);
        }
    }
}