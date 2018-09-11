using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Codecs.Tup;
using Tars.Net.Codecs.Util;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsContentDecoder : IContentDecoder
    {
        public void DecodeRequestContent(Request req)
        {
            if (req.ParameterTypes.Length > 0)
            {
                if (req.Version == TarsCodecsConstant.VERSION)
                {
                    if (req.Parameters == null)
                        req.Parameters = new object[req.ParameterTypes.Length];
                    //todo buffer to byte[] 
                    TarsInputStream jis = new TarsInputStream((byte[])req.Buffer);
                    //todo charset
                    for (int i = 0; i < req.ParameterTypes.Length; i++)
                    {
                        Type type = TarsHelper.GetSourceType(req.ParameterTypes[i].ParameterType);
                        req.Parameters[i] = jis.Read(type, i, false);
                    }
                }
                else if (req.Version == TarsCodecsConstant.VERSION3 || req.Version == TarsCodecsConstant.VERSION2)
                {
                    if (req.Parameters == null)
                        throw new ArgumentException("params name should not be empty");
                    UniAttribute unaIn = new UniAttribute(req.Version);
                    unaIn.Decode((byte[])req.Buffer);
                    for (int i = 0; i < req.ParameterTypes.Length; i++)
                    {
                        req.Parameters[i] = unaIn.GetByType(req.ParameterTypes[i].Name, TarsHelper.GetSourceType(req.ParameterTypes[i].ParameterType));
                    }
                }
                else
                {
                    throw new TarsException(RpcStatusCode.ServerDecodeErr, "un supported protocol, version = " + req.Version);
                }
                req.Buffer = null;
            }
        }

        public void DecodeResponseContent(Response resp)
        {
            TarsInputStream tos = new TarsInputStream((byte[])resp.Buffer);
            var returnType = resp.ReturnValueType.ParameterType;
            if (returnType != typeof(void))
            {
                if (returnType == typeof(Task) || returnType == typeof(ValueType))
                {
                    resp.ReturnValue = Task.CompletedTask;
                }
                else
                {
                    if (returnType.BaseType == typeof(Task))
                    {
                        var resultType = returnType.GenericTypeArguments[0];
                        resp.ReturnValue = Task.FromResult(tos.Read(resultType, 0, true));
                    }
                    else if (returnType.BaseType == typeof(ValueType))
                    {
                        var resultType = returnType.GenericTypeArguments[0];
                        var resultItem = tos.Read(resultType, 0, true);
                        resp.ReturnValue = Activator.CreateInstance(returnType, new object[1] { resultItem });
                    }
                    else
                    {
                        resp.ReturnValue = tos.Read(returnType, 0, true);
                    }
                }
            }
            if (resp.ReturnParameterTypes.Length > 0)
            {
                if (resp.ReturnParameters == null)
                    resp.ReturnParameters = new object[resp.ReturnParameterTypes.Length];
                for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
                {
                    Type type = TarsHelper.GetSourceType(resp.ReturnParameterTypes[i].ParameterType);
                    resp.ReturnParameters[i] = tos.Read(type, i + 1, true);
                }
            }
            resp.Buffer = null;
        }
    }
}
