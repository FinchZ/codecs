using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Codecs.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsEncoder : IEncoder<IByteBuffer>
    {
        //todo RpcConfiguration.
        private string charsetName;

        public TarsEncoder(string charsetName)
        {
            this.charsetName = charsetName;
        }
        public IByteBuffer EncodeRequest(Request req)
        {
            TarsOutputStream tos = new TarsOutputStream();
            tos.SetServerEncoding(charsetName);
            tos.GetByteBuffer().WriteInt(0);
            tos.Write(req.Version, 1);
            tos.Write(req.PacketType, 2);
            tos.Write(req.MessageType, 3);
            tos.Write(TarsCodecsConstant.DEFAULT_TICKET_NUMBER, 4); 
            tos.Write(req.ServantName, 5);
            tos.Write(req.FuncName, 6);
            tos.Write(EncodeRequestContent(req, charsetName), 7);
            tos.Write(req.Timeout, 8);
            tos.Write(req.Context, 9);
            tos.Write(req.Status, 10);
            IByteBuffer buffer = tos.GetByteBuffer();
            int length = buffer.WriterIndex;
            if (length > TarsCodecsConstant.PACKAGE_MAX_LENGTH || length <= 0)
                throw new ProtocolException("the length header of the package must be between 0~10M bytes. data length:" + length);
            tos.ResetDataLength(length);
            return Unpooled.WrappedBuffer(tos.ToByteArray());
        }

        private byte[] EncodeRequestContent(Request request, string charsetName)
        {
            TarsOutputStream tos = new TarsOutputStream();
            tos.SetServerEncoding(charsetName);

            for (int i = 0; i < request.ParameterTypes.Length; i++)
            {
                tos.Write(request.Parameters[i], request.ParameterTypes[i].Position + 1);
            } 
            return tos.ToByteArray();
        }

        public IByteBuffer EncodeResponse(Response message)
        {
            TarsOutputStream tos = new TarsOutputStream();
            tos.SetServerEncoding(charsetName);
            try
            {
                //data length
                tos.GetByteBuffer().WriteInt(0);
                tos.Write(message.Version, 1);
                //保证协议兼容性 PacketType填入默认值 (请求类型：一定是往返请求)
                tos.Write(TarsCodecsConstant.NORMAL, 2);
                if (message.Version == TarsCodecsConstant.VERSION)
                {
                    tos.Write(message.RequestId, 3);
                    tos.Write(message.MessageType, 4);
                    tos.Write(message.ResultStatusCode, 5);
                    //按顺序写入数据流 此处写入 参数 byte[] 
                    tos.Write(EncodeResponseContent(message, charsetName), 6);
                    if (message.Status != null)
                        tos.Write(message.Status, 7);
                    if (message.ResultStatusCode != RpcStatusCode.ServerSuccess)
                        tos.Write((string.IsNullOrWhiteSpace(message.ResultDesc) ? "" : message.ResultDesc), 8);
                }
                else if (message.Version == TarsCodecsConstant.VERSION2 || message.Version == TarsCodecsConstant.VERSION3)
                {
                    tos.Write(message.MessageType, 3);
                    tos.Write(TarsCodecsConstant.DEFAULT_TICKET_NUMBER, 4); 
                    tos.Write(message.ServantName, 5);
                    tos.Write(message.FuncName, 6);
                    //tos.Write(encodeWupResult(), 7);
                    tos.Write(message.Timeout, 8);
                    if (message.Context != null)
                        tos.Write(message.Context, 9);
                    if (message.Status != null)
                        tos.Write(message.Status, 10);
                }
                else
                    message.ResultStatusCode = RpcStatusCode.ServerEncodeErr;
            }
            catch (Exception ex)
            {
                if (message.ResultStatusCode == RpcStatusCode.ServerSuccess)
                    message.ResultStatusCode = RpcStatusCode.ServerEncodeErr;
            }
            IByteBuffer buffer = tos.GetByteBuffer();
            int length = buffer.WriterIndex;
            tos.ResetDataLength(length);
            return Unpooled.WrappedBuffer(tos.ToByteArray());
        }

        private byte[] EncodeResponseContent(Response response, string charsetName)
        {
            TarsOutputStream tos = new TarsOutputStream();
            tos.SetServerEncoding(charsetName);
            var type = response.ReturnValueType.ParameterType;
            if (response.ResultStatusCode == RpcStatusCode.ServerSuccess)
            {
                if (type != typeof(void) && type != typeof(Task))
                    //0的位置是专门给返回值用的
                    tos.Write(response.ReturnValue, 0);
                int outResIndex = 0;
                foreach (var item in response.ReturnParameterTypes)
                {
                    int order = item.Position + 1;
                    tos.Write(response.ReturnParameters[outResIndex++], order);
                }
            }
            return tos.ToByteArray();
        }
    }
}
