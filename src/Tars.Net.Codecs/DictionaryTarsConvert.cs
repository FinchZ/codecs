using DotNetty.Buffers;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public interface IDictionaryInterfaceTarsConvert<K, V> : ITarsConvert<IDictionary<K, V>>
    { }

    public interface IDictionaryClassTarsConvert<K, V> : ITarsConvert<Dictionary<K, V>>
    { }

    public class DictionaryInterfaceTarsConvert<K, V> : TarsConvertBase<IDictionary<K, V>>, IDictionaryInterfaceTarsConvert<K, V>
    {
        private readonly ITarsConvertRoot convert;
        private readonly ITarsHeadHandler headHandler;

        public DictionaryInterfaceTarsConvert(ITarsConvertRoot convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public override IDictionary<K, V> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.Map:
                    {
                        int size = buffer.ReadInt();
                        var dict = new Dictionary<K, V>(size);
                        var op = options.Create();
                        for (int i = 0; i < size; ++i)
                        {
                            headHandler.ReadHead(buffer, op);
                            var k = convert.Deserialize<K>(buffer, op);
                            headHandler.ReadHead(buffer, op);
                            var v = convert.Deserialize<V>(buffer, op);
                            if (dict.ContainsKey(k))
                            {
                                dict[k] = v;
                            }
                            else
                            {
                                dict.Add(k, v);
                            }
                        }
                        return dict;
                    }
                default:
                    throw new TarsDecodeException($"DictionaryTarsConvert can not deserialize {options}");
            }
        }

        public override void Serialize(IDictionary<K, V> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            if (obj != null)
            {
                headHandler.Reserve(buffer, 8);
                headHandler.WriteHead(buffer, TarsStructType.Map, options.Tag);
                buffer.WriteInt(obj.Count);
                foreach (var kv in obj)
                {
                    options.Tag = 0;
                    convert.Serialize(kv.Key, buffer, options);
                    options.Tag = 1;
                    convert.Serialize(kv.Value, buffer, options);
                }
            }
        }
    }

    public class DictionaryClassTarsConvert<K, V> : TarsConvertBase<Dictionary<K, V>>, IDictionaryClassTarsConvert<K, V>
    {
        private readonly IDictionaryInterfaceTarsConvert<K, V> convert;

        public DictionaryClassTarsConvert(IDictionaryInterfaceTarsConvert<K, V> convert)
        {
            this.convert = convert;
        }

        public override Dictionary<K, V> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return convert.Deserialize(buffer, options) as Dictionary<K, V>;
        }

        public override void Serialize(Dictionary<K, V> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convert.Serialize(obj as IDictionary<K, V>, buffer, options);
        }
    }
}