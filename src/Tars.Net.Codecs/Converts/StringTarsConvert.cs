using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class StringTarsConvert : TarsConvertBase<string>
    {
        public StringTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override string DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            order = tag;
            switch (tarsType)
            {
                case TarsStructBase.STRING1:
                    {
                        int len = buffer.ReadByte();
                        return buffer.ReadString(len, options.Encoding);
                    }
                case TarsStructBase.STRING4:
                    {
                        int len = buffer.ReadInt();
                        if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                        {
                            throw new TarsDecodeException("string too long: " + len);
                        }

                        return buffer.ReadString(len, options.Encoding);
                    }
                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void SerializeT(string obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            if (string.IsNullOrEmpty(obj))
            {
                WriteHead(buffer, TarsStructBase.STRING1, order);
                buffer.WriteByte(0);
            }
            else if (obj.Length > 255)
            {
                WriteHead(buffer, TarsStructBase.STRING4, order);
                buffer.WriteInt(obj.Length);
                buffer.WriteString(obj, options.Encoding);
            }
            else
            {
                WriteHead(buffer, TarsStructBase.STRING1, order);
                buffer.WriteByte(obj.Length);
                buffer.WriteString(obj, options.Encoding);
            }
        }
    }
}