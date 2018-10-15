using DotNetty.Buffers;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public interface IListInterfaceTarsConvert<T> : ITarsConvert<IList<T>>
    { }

    public interface IListClassTarsConvert<T> : ITarsConvert<List<T>>
    { }

    public class ListInterfaceTarsConvert<T> : TarsConvertBase<IList<T>>, IListInterfaceTarsConvert<T>
    {
        private readonly ITarsConvert<T> convert;
        private readonly ITarsHeadHandler headHandler;

        public ListInterfaceTarsConvert(ITarsConvert<T> convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override IList<T> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.List:
                    {
                        int size = buffer.ReadInt();
                        var list = new List<T>(size);
                        var op = options.Create();
                        for (int i = 0; i < size; ++i)
                        {
                            headHandler.ReadHead(buffer, op);
                            var t = convert.Deserialize(buffer, op);
                            list.Add(t);
                        }
                        return list;
                    }
                default:
                    throw new TarsDecodeException($"DictionaryTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(IList<T> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj != null)
            {
                headHandler.Reserve(buffer, 8);
                headHandler.WriteHead(buffer, TarsStructType.List, options.Tag);
                buffer.WriteInt(obj.Count);
                var op = options.Create();
                op.Tag = 0;
                foreach (var item in obj)
                {
                    convert.Serialize(item, buffer, op);
                }
            }
        }
    }

    public class ListClassTarsConvert<T> : TarsConvertBase<List<T>>, IListClassTarsConvert<T>
    {
        private readonly IListInterfaceTarsConvert<T> convert;

        public ListClassTarsConvert(IListInterfaceTarsConvert<T> convert)
        {
            this.convert = convert;
        }

        public override List<T> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return convert.Deserialize(buffer, options) as List<T>;
        }

        public override void Serialize(List<T> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convert.Serialize(obj as IList<T>, buffer, options);
        }
    }
}