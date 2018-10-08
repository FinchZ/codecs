using Tars.Net.Codecs.Attributes;
using Xunit;

namespace Tars.Net.Test
{
    public class TarsStructAttributeTest
    {
        [Fact]
        public void ShouldEqualExpect()
        {
            Assert.Equal("Test", new TarsStructAttribute() { Comment = "Test" }.Comment);
            Assert.Equal("Test2", new TarsStructPropertyAttribute(3) { Comment = "Test2" }.Comment);
            Assert.Equal(13, new TarsStructPropertyAttribute(13) { Comment = "Test2" }.Order);
        }
    }
}