using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsContentDecoder : IContentDecoder
    {
        public void DecodeRequestContent(Request req)
        {
            if (req.ParameterTypes.Length > 0)
            {
                if (req.Parameters == null)
                    req.Parameters = new object[req.ParameterTypes.Length];
                //todo buffer to byte[] 
                TarsInputStream jis = new TarsInputStream((byte[])req.Buffer);
                //todo charset
                for (int i = 0; i < req.ParameterTypes.Length; i++)
                {
                    Type type = req.ParameterTypes[i].ParameterType;
                    req.Parameters[i] = jis.Read(type, i, false);
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
                if (returnType == typeof(Task))
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
                    resp.ReturnParameters[i] = tos.Read(resp.ReturnParameterTypes[i].ParameterType, i + 1, true);
                }
            }
            resp.Buffer = null;
        }
    }
}
