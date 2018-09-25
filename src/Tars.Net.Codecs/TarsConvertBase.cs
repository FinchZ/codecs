using DotNetty.Buffers;

namespace Tars.Net.Codecs
{
    public abstract class TarsConvertBase<T> : ITarsConvert<T>
    {
        public virtual bool Accept(Codec codec, short version)
        {
            return codec == Codec.Tars;
        }

        public void Reserve(IByteBuffer buffer, int len)
        {
            buffer.EnsureWritable(len, true);
        }

        public void ReadHead(IByteBuffer buffer, TarsConvertOptions options)
        {
            byte b = buffer.ReadByte();
            byte tarsType = (byte)(b & 15);
            int tag = ((b & (15 << 4)) >> 4);
            //var tagType = TagType.Tag1;
            if (tag == 15)
            {
                tag = buffer.ReadByte() & 0x00ff;
                //tagType = TagType.Tag2;
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

        public abstract T Deserialize(IByteBuffer buffer, TarsConvertOptions options);

        public abstract void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options);
    }
}