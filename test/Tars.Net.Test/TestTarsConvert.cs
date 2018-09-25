using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Codecs;

namespace Tars.Net.Test
{
    public static class TestTarsConvert
    {
        static TestTarsConvert()
        {
            Provider = new ServiceCollection()
                .AddTarsCodecs()
                .BuildServiceProvider();
            ConvertRoot = Provider.GetRequiredService<ITarsConvertRoot>();
            HeadHandler = Provider.GetRequiredService<ITarsHeadHandler>();
        }

        public static ServiceProvider Provider { get; }
        public static ITarsConvertRoot ConvertRoot { get; }
        public static ITarsHeadHandler HeadHandler { get; }
    }
}