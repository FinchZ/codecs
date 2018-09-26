using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ByteBufferTarsConvert : TarsConvertBase<IByteBuffer>
    {
        private readonly ITarsHeadHandler headHandler;

        public ByteBufferTarsConvert(ITarsHeadHandler headHandler)
        {
            this.headHandler = headHandler;
        }

        public override IByteBuffer Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.ByteArray:
                    var tag = options.Tag;
                    var type = options.TarsType;
                    headHandler.ReadHead(buffer, options);
                    if (options.TarsType != TarsStructType.Byte)
                    {
                        throw new TarsDecodeException($"type mismatch, tag: {tag}, type: {options.TarsType},{type}");
                    }

                    int size = buffer.ReadInt();
                    if (size < 0)
                    {
                        throw new TarsDecodeException($"invalid size tag: {tag}, type: {options.TarsType},{type}, size: {size}");
                    }

                    return buffer.ReadBytes(size);

                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(IByteBuffer obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            int len = obj.ReadableBytes;
            headHandler.Reserve(buffer, 8 + len);
            headHandler.WriteHead(buffer, TarsStructType.ByteArray, options.Tag);
            headHandler.WriteHead(buffer, TarsStructType.Byte, 0);
            buffer.WriteInt(len);
            buffer.WriteBytes(obj);
        }
    }
}