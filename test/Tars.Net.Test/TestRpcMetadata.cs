using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Test
{
    public class TestRpcMetadata : IRpcMetadata
    {
        public IEnumerable<Type> Clients => throw new NotImplementedException();

        public IEnumerable<(Type service, Type implementation)> RpcServices => throw new NotImplementedException();

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName)
        {
            return TestTarsConvert.FindRpcMethod(servantName, funcName);
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