using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ByteArrayTarsConvert : TarsConvertBase<byte[]>
    {
        private readonly ITarsHeadHandler headHandler;

        public ByteArrayTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override byte[] Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.ByteArray:
                    headHandler.ReadHead(buffer, options);
                    int size = buffer.ReadInt();
                    var bytes = new byte[size];
                    buffer.ReadBytes(bytes);
                    return bytes;

                default:
                    throw new TarsDecodeException($"ByteArrayTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(byte[] obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj == null)
            {
                return;
            }

            int len = obj.Length;
            headHandler.Reserve(buffer, 8 + len);
            headHandler.WriteHead(buffer, TarsStructType.ByteArray, options.Tag);
            headHandler.WriteHead(buffer, TarsStructType.Byte, 0);
            buffer.WriteInt(len);
            buffer.WriteBytes(obj);
        }
    }
}