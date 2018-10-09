using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Test
{
    public static class TestTarsConvert
    {
        static TestTarsConvert()
        {
            Provider = new ServiceCollection()
                .AddSingleton<IRpcMetadata>(new TestRpcMetadata())
                .AddTarsCodecs()
                .BuildServiceProvider();
            ConvertRoot = Provider.GetRequiredService<ITarsConvertRoot>();
            HeadHandler = Provider.GetRequiredService<ITarsHeadHandler>();
        }

        public static ServiceProvider Provider { get; }
        public static ITarsConvertRoot ConvertRoot { get; }
        public static ITarsHeadHandler HeadHandler { get; }
        public static Func<string, string, (MethodInfo , bool , ParameterInfo[] , Codec , short , Type )> FindRpcMethodFunc { get; set; }

        public static (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName)
        {
            return FindRpcMethodFunc(servantName, funcName);
        }
    }
}