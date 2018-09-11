using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs.Tup
{
    public class UniAttribute
    {
        public UniAttribute(short version) : this("UTF-8", version) { }

        public UniAttribute(string encodeName, short version)
        {
            EncodeName = encodeName;
            if (version == TarsCodecsConstant.VERSION3)
            {
                newData = new Dictionary<string, byte[]>();
                IsVersionTup3 = true;
            }
            else
                data = new Dictionary<string, Dictionary<string, byte[]>>();
        }

        /// <summary>
        /// TUP2 ParamName-Type-Value
        /// </summary>
        protected Dictionary<string, Dictionary<string, byte[]>> data;

        /// <summary>
        ///  TUP3 ParamName-Value
        /// </summary>
        protected Dictionary<string, byte[]> newData;
        /// <summary>
        /// 存储get后的数据 避免多次解析
        /// </summary>
        private Dictionary<string, object> cachedData = new Dictionary<string, object>(128);


        public string EncodeName { get; set; } = "UTF-8";

        public bool IsVersionTup3 = false;


        public void Put<T>(string name, T t)
        {
            if (name == null)
                throw new ArgumentException("put key can not be null");
            if (t == null)
                throw new ArgumentException("put value can not be null");
            TarsOutputStream _out = new TarsOutputStream();
            _out.SetServerEncoding(EncodeName);
            _out.Write(t, 0);
            byte[] sBuffer = _out.ToByteArray();
            if (IsVersionTup3)
                if (newData.ContainsKey(name))
                    newData[name] = sBuffer;
                else
                    newData.Add(name, sBuffer);
            else
            {
                List<string> listType = new List<string>();
                CheckObjectType(listType, t);
                string className = BasicClassTypeUtil.TransTypeList(listType);
                Dictionary<string, byte[]> pair = new Dictionary<string, byte[]>(1) { { className, sBuffer } };
                cachedData.Remove(name);
                if (data.ContainsKey(name))
                    data[name] = pair;
                else
                    data.Add(name, pair);
            }
        }

        internal object GetByType(string name, Type type)
        {
            if (cachedData.ContainsKey(name))
                return cachedData[name];
            object result = null;
            if (IsVersionTup3)
            {
                if (!newData.ContainsKey(name))
                    return null;
                else
                    result = DecodeBuffer(newData[name], type);
            }
            else
            {
                if (!data.ContainsKey(name))
                    return null;
                else
                {
                    var pair = data[name];
                    foreach (var item in pair)
                    {
                        result = DecodeBuffer(newData[name], type);
                        break;
                    }
                } 
            } 
            cachedData[name] = result;
            return result;
        }

        private object DecodeBuffer(byte[] buffer, Type type)
        {
            TarsInputStream tis = new TarsInputStream(buffer);
            tis.SetServerEncoding(EncodeName);
            var result = tis.Read(type, 0, true);
            return result;
        }

        internal void Decode(byte[] buffer)
        {
            TarsInputStream tis = new TarsInputStream(buffer);
            tis.SetServerEncoding(EncodeName);
            if (IsVersionTup3)
                tis.ReadMap(newData, 0, false);
            else
                tis.ReadMap(data, 0, false);
        }

        private void CheckObjectType(List<string> listType, Object t)
        {
            Type type = t.GetType();
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (!elementType.Equals(TarsSupportBaseType.BYTE))
                    throw new ArgumentException("only byte[] is supported");
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
                        CheckObjectType(listType, list[0]);
                    else
                        listType.Add("?");
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
                    throw new ArgumentException("only Dictionary<K,V>,List<T> generic type supported");
            }
            else
            {
                listType.Add(type.Name);
            }
        }

        public byte[] Encode()
        {
            TarsOutputStream os = new TarsOutputStream();
            os.SetServerEncoding(EncodeName);
            if (IsVersionTup3)
            {
                os.Write((IDictionary)newData, 0);
                return os.ToByteArray();
            }
            else
            {
                os.Write((IDictionary)data, 0);
                return os.ToByteArray();
            }
        }
    }
}
