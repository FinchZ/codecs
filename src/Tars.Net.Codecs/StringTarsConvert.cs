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
                        return buffer.ReadString(len, options.Encoding);
                    }
                default:
                    throw new TarsDecodeException($"StringTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(string obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj == null)
            {
                return;
            }
            var count = options.Encoding.GetByteCount(obj);
            if (count > 255)
            {
                headHandler.WriteHead(buffer, TarsStructType.String4, options.Tag);
                buffer.WriteInt(count);
                buffer.WriteString(obj, options.Encoding);
            }
            else
            {
                headHandler.WriteHead(buffer, TarsStructType.String1, options.Tag);
                buffer.WriteByte(count);
                buffer.WriteString(obj, options.Encoding);
            }
        }
    }
}