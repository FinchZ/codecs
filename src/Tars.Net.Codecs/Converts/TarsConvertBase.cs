using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tars.Net.Codecs
{
    public abstract class TarsConvertBase : ITarsConvert
    {
        protected readonly ITarsConvertRoot convertRoot;

        public TarsConvertBase(IServiceProvider provider)
        {
            convertRoot = provider.GetRequiredService<ITarsConvertRoot>();
        }

        public virtual int Order => 0;

        public abstract bool Accept((Type, short) options);

        public abstract object Deserialize(IByteBuffer buffer, Type type, int order, bool isRequire = true, TarsConvertOptions options = null);

        public abstract void Serialize(object obj, IByteBuffer buffer, int order, bool isRequire = true, TarsConvertOptions options = null);

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

        public (byte tarsType, int tag, TagType tagType) ReadHead(IByteBuffer buffer, HeadData hd)
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
    }

    public abstract class TarsConvertBase<T> : TarsConvertBase
    {
        public TarsConvertBase(IServiceProvider provider) : base(provider)
        {
        }

        public override bool Accept((Type, short) options)
        {
            return options.Item1 == typeof(T) && AcceptVersion(options.Item2);
        }

        public abstract bool AcceptVersion(short version);

        public override object Deserialize(IByteBuffer buffer, Type type, int order, bool isRequire = true, TarsConvertOptions options = null)
        {
            return DeserializeT(buffer, order, isRequire, options);
        }

        public override void Serialize(object obj, IByteBuffer buffer, int order, bool isRequire = true, TarsConvertOptions options = null)
        {
            SerializeT((T)obj, buffer, order, isRequire, options);
        }

        public abstract T DeserializeT(IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options);

        public abstract void SerializeT(T obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options);
    }
}