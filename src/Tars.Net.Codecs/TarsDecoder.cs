using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Codecs.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class TarsDecoder : IDecoder<IByteBuffer>
    {
        public Request DecodeRequest(IByteBuffer input)
        {
            if (input.Capacity < TarsCodecsConstant.HEAD_SIZE)
                return null;
            int length = input.ReadInt() - TarsCodecsConstant.HEAD_SIZE;
            if (length > TarsCodecsConstant.PACKAGE_MAX_LENGTH || length <= 0)
                throw new ProtocolException("the length header of the package must be between 0~10M bytes. data length:" + length);
            if (input.Capacity < length)
                return null;

            TarsInputStream jis = new TarsInputStream(input);
            Request request = new Request();
            request.Version = jis.ReadShort(1, true);
            request.PacketType = jis.ReadByte(2, true);
            request.MessageType = jis.ReadInt(3, true);
            request.RequestId = jis.ReadInt(4, true);
            request.ServantName = jis.ReadString(5, true);
            request.FuncName = jis.ReadString(6, true);
            request.Buffer = jis.ReadByteArray(7, true);//数据
            request.Timeout = jis.ReadInt(8, true);//超时时间
            request.Context = jis.ReadMap(9, true);
            request.Status = jis.ReadMap(10, true);
            return request;
        }

        public Response DecodeResponse(IByteBuffer input)
        {
            if (input.Capacity < TarsCodecsConstant.HEAD_SIZE)
                return null;
            TarsInputStream tos = new TarsInputStream(input);
            int length = tos.GetDataLength() - TarsCodecsConstant.HEAD_SIZE;
            if (length > TarsCodecsConstant.PACKAGE_MAX_LENGTH || length <= 0)
                throw new ProtocolException("the length header of the package must be between 0~10M bytes. data length:" + length);
            //校验数据
            if (input.Capacity - input.ReaderIndex < length)
                return null;
            Response response = new Response();
            response.Version = tos.ReadShort(1, true);
            int packetType = tos.ReadByte(2, true);
            response.RequestId = tos.ReadInt(3, true);
            response.MessageType = tos.ReadInt(4, true);
            response.ResultStatusCode = (RpcStatusCode)tos.ReadInt(5, true);
            if (response.ResultStatusCode == RpcStatusCode.ServerSuccess)
                response.Buffer = tos.ReadByteArray(6, true);
            response.Status = tos.ReadMap(7, false);
            if (response.ResultStatusCode != RpcStatusCode.ServerSuccess)
                response.ResultDesc = tos.ReadString(8, false);
            return response;
        }
    }
}
