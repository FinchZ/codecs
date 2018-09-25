using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public interface ITarsHeadHandler
    {
        void Reserve(IByteBuffer buffer, int len);

        void ReadHead(IByteBuffer buffer, TarsConvertOptions options);

        void WriteHead(IByteBuffer buffer, byte type, int tag);
    }

    public class TarsHeadHandler : ITarsHeadHandler
    {
        public void Reserve(IByteBuffer buffer, int len)
        {
            buffer.EnsureWritable(len, true);
        }

        public void ReadHead(IByteBuffer buffer, TarsConvertOptions options)
        {
            byte b = buffer.ReadByte();
            byte tarsType = (byte)(b & 15);
            int tag = ((b & (15 << 4)) >> 4);
            if (tag == 15)
            {
                tag = buffer.ReadByte() & 0x00ff;
            }
            options.Tag = tag;
            options.TarsType = tarsType;
        }

        public void WriteHead(IByteBuffer buffer, byte type, int tag)
        {
            if (tag < 15)
            {
                byte b = (byte)((tag << 4) | type);
                buffer.WriteByte(b);
            }
            else if (tag < 256)
            {
                byte b = (byte)((15 << 4) | type);
                buffer.WriteByte(b);
                buffer.WriteByte(tag);
            }
            else
            {
                throw new TarsEncodeException("tag is too large: " + tag);
            }
        }
    }
}