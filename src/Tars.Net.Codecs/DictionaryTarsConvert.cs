using DotNetty.Buffers;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public interface IDictionaryTarsConvert<K, V> : ITarsConvert<IDictionary<K, V>>
    { }

    public class DictionaryTarsConvert<K, V> : TarsConvertBase<IDictionary<K, V>>, IDictionaryTarsConvert<K, V>
    {
        private readonly ITarsConvert<K> kConvert;
        private readonly ITarsConvert<V> vConvert;
        private readonly ITarsHeadHandler headHandler;

        public DictionaryTarsConvert(ITarsConvert<K> kConvert, ITarsConvert<V> vConvert, ITarsHeadHandler headHandler)
        {
            this.kConvert = kConvert;
            this.vConvert = vConvert;
            this.headHandler = headHandler;
        }

        public override IDictionary<K, V> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructType.MAP:
                    {
                        int size = buffer.ReadInt();
                        if (size < 0)
                        {
                            throw new TarsDecodeException("size invalid: " + size);
                        }

                        var dict = new Dictionary<K, V>(size);
                        for (int i = 0; i < size; ++i)
                        {
                            headHandler.ReadHead(buffer, options);
                            var k = kConvert.Deserialize(buffer, options);
                            headHandler.ReadHead(buffer, options);
                            var v = vConvert.Deserialize(buffer, options);
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
                    throw new TarsDecodeException("type mismatch.");
            }
        }

        public override void Serialize(IDictionary<K, V> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            headHandler.Reserve(buffer, 8);
            headHandler.WriteHead(buffer, TarsStructType.MAP, options.Tag);
            if (obj == null)
            {
                buffer.WriteInt(0);
            }
            else
            {
                buffer.WriteInt(obj.Count);
                foreach (var kv in obj)
                {
                    options.Tag = 0;
                    kConvert.Serialize(kv.Key, buffer, options);
                    options.Tag = 1;
                    vConvert.Serialize(kv.Value, buffer, options);
                }
            }
        }
    }
}