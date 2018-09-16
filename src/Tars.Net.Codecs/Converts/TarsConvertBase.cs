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

        public virtual Codec Codec => Codec.Tars;

        public abstract object Deserialize(IByteBuffer buffer, Type type, out int order, TarsConvertOptions options = null);

        public abstract void Serialize(object obj, IByteBuffer buffer, int order, bool isRequire = true, TarsConvertOptions options = null);

        public virtual bool Accept((Codec, Type, short) options)
        {
            return options.Item1 == Codec.Tars && AcceptT(options.Item2, options.Item3);
        }

        public abstract bool AcceptT(Type type, short version);

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
    }

    public abstract class TarsConvertBase<T> : TarsConvertBase
    {
        public TarsConvertBase(IServiceProvider provider) : base(provider)
        {
        }

        public override bool AcceptT(Type type, short version)
        {
            return type == typeof(T) && AcceptVersion(version);
        }

        public virtual bool AcceptVersion(short version)
        {
            return true;
        }

        public override object Deserialize(IByteBuffer buffer, Type type, out int order, TarsConvertOptions options = null)
        {
            return DeserializeT(buffer, out order, options);
        }

        public override void Serialize(object obj, IByteBuffer buffer, int order, bool isRequire = true, TarsConvertOptions options = null)
        {
            SerializeT((T)obj, buffer, order, isRequire, options);
        }

        public abstract T DeserializeT(IByteBuffer buffer, out int order, TarsConvertOptions options);

        public abstract void SerializeT(T obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options);
    }
}