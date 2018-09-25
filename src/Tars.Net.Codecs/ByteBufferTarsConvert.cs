using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public class ByteBufferTarsConvert : TarsConvertBase<IByteBuffer>
    {
        public override IByteBuffer Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.SIMPLE_LIST:
                    var tag = options.Tag;
                    var type = options.TarsType;
                    ReadHead(buffer, options);
                    if (options.TarsType != TarsStructType.BYTE)
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
            Reserve(buffer, 8 + len);
            WriteHead(buffer, TarsStructType.SIMPLE_LIST, options.Tag);
            WriteHead(buffer, TarsStructType.BYTE, 0);
            buffer.WriteInt(len);
            buffer.WriteBytes(obj);
        }
    }
}