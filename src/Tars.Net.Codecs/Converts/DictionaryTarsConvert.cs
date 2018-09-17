using DotNetty.Buffers;
using System;
using System.Collections.Generic;

namespace Tars.Net.Codecs.Converts
{
    public interface IDictionaryTarsConvert<K, V> : ITarsConvert<IDictionary<K, V>>
    { }

    public class DictionaryTarsConvert<K, V> : TarsConvertBase<IDictionary<K, V>>, IDictionaryTarsConvert<K, V>
    {
        public DictionaryTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override IDictionary<K, V> Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            switch (options.TarsType)
            {
                case TarsStructBase.MAP:
                    {
                        int size = buffer.ReadInt();
                        if (size < 0)
                        {
                            throw new TarsDecodeException("size invalid: " + size);
                        }

                        var dict = new Dictionary<K, V>(size);
                        for (int i = 0; i < size; ++i)
                        {
                            convertRoot.ReadHead(buffer, options);
                            var k = convertRoot.Deserialize<K>(buffer, options);
                            convertRoot.ReadHead(buffer, options);
                            var v = convertRoot.Deserialize<V>(buffer, options);
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
            Reserve(buffer, 8);
            WriteHead(buffer, TarsStructBase.MAP, options.Tag);
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
                    convertRoot.Serialize(kv.Key, buffer, options);
                    options.Tag = 1;
                    convertRoot.Serialize(kv.Key, buffer, options);
                }
            }
        }
    }
}