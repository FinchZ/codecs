﻿using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Test
{
    public class TestTarsConvert
    {
        public TestTarsConvert()
        {
            Provider = new ServiceCollection()
                .AddSingleton<IRpcMetadata>(new TestRpcMetadata(this))
                .AddSingleton<IClientCallBack>(new TestClientCallBack(this))
                .AddTarsCodecs()
                .BuildServiceProvider();
            ConvertRoot = Provider.GetRequiredService<ITarsConvertRoot>();
            HeadHandler = Provider.GetRequiredService<ITarsHeadHandler>();
        }

        public (string servantName, string funcName)? FindRpcMethod(int callBackId)
        {
            return FindRpcMethodByIdFunc(callBackId);
        }

        public ServiceProvider Provider { get; }
        public ITarsConvertRoot ConvertRoot { get; }
        public ITarsHeadHandler HeadHandler { get; }
        public Func<string, string, (MethodInfo , bool , ParameterInfo[] , Codec , short , Type )> FindRpcMethodFunc { get; set; }
        public Func<int, (string servantName, string funcName)?> FindRpcMethodByIdFunc { get; set; }

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName)
        {
            return FindRpcMethodFunc(servantName, funcName);
        }
    }
}