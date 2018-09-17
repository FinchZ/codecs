using DotNetty.Buffers;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public interface IListTarsConvert<T> : ITarsConvert<IList<T>>
    { }

    public class ListTarsConvert<T> : TarsConvertBase<IList<T>>, IListTarsConvert<T>
    {
        private readonly ITarsConvert<T> convert;

        public ListTarsConvert(ITarsConvert<T> convert)
        {
            this.convert = convert;
        }

        public override IList<T> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.LIST:
                    {
                        int size = buffer.ReadInt();
                        if (size < 0)
                        {
                            throw new TarsDecodeException("size invalid: " + size);
                        }

                        var list = new List<T>(size);
                        for (int i = 0; i < size; ++i)
                        {
                            ReadHead(buffer, options);
                            var t = convert.Deserialize(buffer, options);
                            list.Add(t);
                        }
                        return list;
                    }
                default:
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(IList<T> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            Reserve(buffer, 8);
            WriteHead(buffer, TarsStructBase.LIST, options.Tag);
            if (obj == null)
            {
                buffer.WriteInt(0);
            }
            else
            {
                buffer.WriteInt(obj.Count);
                foreach (var item in obj)
                {
                    convert.Serialize(item, buffer, options);
                }
            }
        }
    }
}