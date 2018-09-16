using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class StringTarsConvert : TarsConvertBase<string>
    {
        public StringTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, string value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (tarsType, tag, tagType) = ReadHead(buffer);
            switch (tarsType)
            {
                case TarsStructBase.STRING1:
                    {
                        int len = buffer.ReadByte();
                        return (tag, buffer.ReadString(len, options.Encoding));
                    }
                case TarsStructBase.STRING4:
                    {
                        int len = buffer.ReadInt();
                        if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                        {
                            throw new TarsDecodeException("string too long: " + len);
                        }

                        return (tag, buffer.ReadString(len, options.Encoding));
                    }
                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(string obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
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