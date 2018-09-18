using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs.Converts
{
    public class UniAttributeV2
    {
        private IDictionary<string, IDictionary<string, IByteBuffer>> data = new Dictionary<string, IDictionary<string, IByteBuffer>>();

        /// <summary>
        /// 存储get后的数据 避免多次解析
        /// </summary>
        private readonly Dictionary<string, object> cachedData = new Dictionary<string, object>(128);

        private readonly ITarsConvertRoot convert;

        public UniAttributeV2(ITarsConvertRoot convert)
        {
            this.convert = convert;
        }

        public void Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = convert.Deserialize<IByteBuffer>(buffer, options);
            data = convert.Deserialize<IDictionary<string, IDictionary<string, IByteBuffer>>>(buf, options);
        }

        public void Serialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(data, buf, options);
            convert.Serialize(buf, buffer, options);
        }

        public void Put<T>(string name, T obj, TarsConvertOptions options)
        {
            if (name == null)
            {
                throw new ArgumentException("put key can not be null");
            }

            if (obj == null)
            {
                throw new ArgumentException("put value can not be null");
            }

            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(obj, buf, options);
            List<string> listType = new List<string>();
            CheckObjectType(listType, obj);
            string className = BasicClassTypeUtil.TransTypeList(listType);
            Dictionary<string, IByteBuffer> pair = new Dictionary<string, IByteBuffer>(1) { { className, buf } };
            cachedData.Remove(name);
            if (data.ContainsKey(name))
            {
                data[name] = pair;
            }
            else
            {
                data.Add(name, pair);
            }
        }

        public void CheckObjectType(List<string> listType, Object t)
        {
            Type type = t.GetType();
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (!elementType.Equals(TarsSupportBaseType.BYTE))
                {
                    throw new ArgumentException("only byte[] is supported");
                }

                byte[] temp = t as byte[];
                if (temp.Length > 0)
                {
                    listType.Add("list");
                    CheckObjectType(listType, temp[0]);
                }
                else
                {  //空数组
                    listType.Add("Array");
                    listType.Add("?");
                }
            }
            else if (type.IsGenericType)
            {
                if (type.Equals(TarsSupportBaseType.LIST))
                {
                    listType.Add("list");
                    List<Object> list = t as List<Object>;
                    if (list.Count > 0)
                    {
                        CheckObjectType(listType, list[0]);
                    }
                    else
                    {
                        listType.Add("?");
                    }
                }
                else if (type.Equals(TarsSupportBaseType.MAP))
                {
                    listType.Add("map");
                    Dictionary<object, object> map = t as Dictionary<object, object>;
                    if (map.Count > 0)
                    {
                        foreach (var item in map)
                        {
                            listType.Add(item.Key.GetType().Name);
                            CheckObjectType(listType, item.Value);
                            break;
                        }
                    }
                    else
                    {
                        listType.Add("?");
                        listType.Add("?");
                    }
                }
                else
                {
                    throw new ArgumentException("only Dictionary<K,V>,List<T> generic type supported");
                }
            }
            else
            {
                listType.Add(type.Name);
            }
        }
    }
}