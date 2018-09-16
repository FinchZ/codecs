using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tars.Net.Codecs
{
    public abstract class TarsConvertBase<T> : ITarsConvert<T>
    {
        protected readonly ITarsConvertRoot convertRoot;

        public TarsConvertBase(IServiceProvider provider)
        {
            convertRoot = provider.GetRequiredService<ITarsConvertRoot>();
        }

        public virtual bool Accept(Codec codec, short version)
        {
            return codec == Codec.Tars;
        }

        public void Reserve(IByteBuffer buffer, int len)
        {
            buffer.EnsureWritable(len, true);
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

        public (byte tarsType, int tag, TagType tagType) ReadHead(IByteBuffer buffer)
        {
            byte b = buffer.ReadByte();
            byte tarsType = (byte)(b & 15);
            int tag = ((b & (15 << 4)) >> 4);
            var tagType = TagType.Tag1;
            if (tag == 15)
            {
                tag = buffer.ReadByte() & 0x00ff;
                tagType = TagType.Tag2;
            }
            return (tarsType, tag, tagType);
        }

        public abstract (int order, T value) Deserialize(IByteBuffer buffer, TarsConvertOptions options);

        public abstract void Serialize(T obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options);
    }
}