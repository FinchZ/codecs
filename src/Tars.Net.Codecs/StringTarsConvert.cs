using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class StringTarsConvert : TarsConvertBase<string>
    {
        private readonly ITarsHeadHandler headHandler;

        public StringTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override string Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.String1:
                    {
                        int len = buffer.ReadByte();
                        return buffer.ReadString(len, options.Encoding);
                    }
                case TarsStructType.String4:
                    {
                        int len = buffer.ReadInt();
                        if (len > TarsStructType.MaxStringLength || len < 0)
                        {
                            throw new TarsDecodeException("string too long: " + len);
                        }

                        return buffer.ReadString(len, options.Encoding);
                    }
                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(string obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (string.IsNullOrEmpty(obj))
            {
                headHandler.WriteHead(buffer, TarsStructType.String1, options.Tag);
                buffer.WriteByte(0);
            }
            else if (obj.Length > 255)
            {
                headHandler.WriteHead(buffer, TarsStructType.String4, options.Tag);
                buffer.WriteInt(obj.Length);
                buffer.WriteString(obj, options.Encoding);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.String1, options.Tag);
                buffer.WriteByte(obj.Length);
                buffer.WriteString(obj, options.Encoding);
            }
        }
    }
}