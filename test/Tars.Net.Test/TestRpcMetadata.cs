using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Test
{
    public class TestRpcMetadata : IRpcMetadata
    {
        private readonly TestTarsConvert tarsConvert;

        public TestRpcMetadata(TestTarsConvert tarsConvert)
        {
            this.tarsConvert = tarsConvert;
        }

        public IEnumerable<Type> Clients => throw new NotImplementedException();

        public IEnumerable<(Type service, Type implementation)> RpcServices => throw new NotImplementedException();

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName)
        {
            return tarsConvert.FindRpcMethod(servantName, funcName);
        }

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, string servantName, string funcName, ParameterInfo[] allParameters) FindRpcMethod(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public void Init(IServiceProvider provider)
        {
            throw new NotImplementedException();
        }

        public bool IsRpcClientType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRpcServiceType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}