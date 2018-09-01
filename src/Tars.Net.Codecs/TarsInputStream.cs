using DotNetty.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs
{
    public class TarsInputStream
    {
        private IByteBuffer buffer;

        protected Encoding sServerEncoding = Encoding.GetEncoding("utf-8");

        public void SetServerEncoding(string name)
        {
            sServerEncoding = Encoding.GetEncoding(name);
        }

        public TarsInputStream(IByteBuffer input)
        {
            buffer = input;
        }
        public TarsInputStream(byte[] bs) : this(bs, 0) { }

        public TarsInputStream(byte[] bs, int pos)
        {
            buffer = Unpooled.WrappedBuffer(bs);
            buffer.SkipBytes(pos);
        }
        public void Wrap(byte[] bs) =>
            buffer = Unpooled.WrappedBuffer(bs);

        /// <summary>
        /// 读取数据头
        /// </summary>
        /// <param name="hd"></param>
        /// <param name="bb"></param>
        /// <returns></returns>
        public int ReadHead(HeadData hd, IByteBuffer bb)
        {
            byte b = bb.ReadByte();
            hd.Type = (byte)(b & 15);
            hd.Tag = ((b & (15 << 4)) >> 4);
            if (hd.Tag == 15)
            {
                hd.Tag = bb.ReadByte() & 0x00ff;
                return 2;
            }
            return 1;
        }
        public int ReadHead(HeadData hd) => ReadHead(hd, buffer);
        /// <summary>
        /// 读取头信息，但不移动缓冲区的当前偏移
        /// </summary>
        /// <param name="hd"></param>
        /// <returns></returns>
        private int PeakHead(HeadData hd)
        {
            int len = ReadHead(hd);
            Skip(-1 * len);
            return len;
        }

        /// <summary>
        /// 跳过若干字节
        /// </summary>
        /// <param name="len"></param>
        private void Skip(int len) =>
            buffer.SetReaderIndex(buffer.ReaderIndex + len);

        /// <summary>
        /// 跳到指定的tag的数据之前
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool SkipToTag(int tag)
        {
            try
            {
                var hd = new HeadData();
                while (true)
                {
                    int len = PeakHead(hd);
                    if (hd.Type == TarsStructBase.STRUCT_END) return false;
                    if (tag <= hd.Tag)
                        return tag == hd.Tag;
                    Skip(len);
                    SkipField(hd.Type);
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }
            catch (TarsDecodeException e)
            {
            }
            return false;
        }

        /// <summary>
        /// 跳到当前结构体的结束位置
        /// </summary>
        public void SkipToStructEnd()
        {
            var hd = new HeadData();
            do
            {
                ReadHead(hd);
                SkipField(hd.Type);
            } while (hd.Type != TarsStructBase.STRUCT_END);
        }

        /// <summary>
        /// 跳过一个字段
        /// </summary>
        private void SkipField()
        {
            var hd = new HeadData();
            ReadHead(hd);
            SkipField(hd.Type);
        }

        private void SkipField(byte type)
        {
            switch (type)
            {
                case TarsStructBase.BYTE:
                    Skip(1);
                    break;
                case TarsStructBase.SHORT:
                    Skip(2);
                    break;
                case TarsStructBase.INT:
                    Skip(4);
                    break;
                case TarsStructBase.LONG:
                    Skip(8);
                    break;
                case TarsStructBase.FLOAT:
                    Skip(4);
                    break;
                case TarsStructBase.DOUBLE:
                    Skip(8);
                    break;
                case TarsStructBase.STRING1:
                    {
                        int len = buffer.ReadByte();
                        if (len < 0)
                            len += 256;
                        Skip(len);
                        break;
                    }
                case TarsStructBase.STRING4:
                    {
                        Skip(buffer.ReadInt());
                        break;
                    }
                case TarsStructBase.MAP:
                    {
                        int size = Read(0, 0, true);
                        for (int i = 0; i < size * 2; ++i)
                        {
                            SkipField();
                        }
                        break;
                    }
                case TarsStructBase.LIST:
                    {
                        int size = Read(0, 0, true);
                        for (int i = 0; i < size; ++i)
                        {
                            SkipField();
                        }
                        break;
                    }
                case TarsStructBase.SIMPLE_LIST:
                    {
                        var hd = new HeadData();
                        ReadHead(hd);
                        if (hd.Type != (TarsStructBase.BYTE))
                        {
                            throw new TarsDecodeException("skipField with invalid type, type value: " + type + ", " + hd.Type);
                        }
                        int size = Read(0, 0, true);
                        Skip(size);
                        break;
                    }
                case TarsStructBase.STRUCT_BEGIN:
                    SkipToStructEnd();
                    break;

                case TarsStructBase.STRUCT_END:
                case TarsStructBase.ZERO_TAG:
                    break;
                default:
                    throw new TarsDecodeException("invalid type.");
            }
        }

        public bool Read(bool b, int tag, bool isRequire)
        {
            byte c = Read((byte)0x0, tag, isRequire);
            return c != 0;
        }
        public byte Read(byte c, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        c = 0x0;
                        break;
                    case TarsStructBase.BYTE:
                        {
                            c = buffer.ReadByte();
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return c;
        }

        public short Read(short n, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        n = 0;
                        break;
                    case TarsStructBase.BYTE:
                        {
                            n = (short)buffer.ReadByte();
                            break;
                        }
                    case TarsStructBase.SHORT:
                        {
                            n = buffer.ReadShort();
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return n;
        }

        public int Read(int n, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        n = 0;
                        break;

                    case TarsStructBase.BYTE:
                        n = (int)buffer.ReadByte();
                        break;

                    case TarsStructBase.SHORT:
                        n = buffer.ReadShort();
                        break;

                    case TarsStructBase.INT:
                        n = buffer.ReadInt();
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return n;
        }

        public long Read(long n, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        n = 0;
                        break;
                    case TarsStructBase.BYTE:
                        n = (int)buffer.ReadByte();
                        break;
                    case TarsStructBase.SHORT:
                        n = buffer.ReadShort();
                        break;
                    case TarsStructBase.INT:
                        n = buffer.ReadInt();
                        break;
                    case TarsStructBase.LONG:
                        n = buffer.ReadLong();
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return n;
        }
        public float Read(float n, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        n = 0;
                        break;

                    case TarsStructBase.FLOAT:
                        n = buffer.ReadFloat();
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return n;
        }

        public double Read(double n, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        n = 0;
                        break;
                    case TarsStructBase.FLOAT:
                        n = buffer.ReadFloat();
                        break;

                    case TarsStructBase.DOUBLE:
                        n = buffer.ReadDouble();
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return n;
        }

        public string ReadByteString(string s, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.STRING1:
                        {
                            int len = buffer.ReadByte();
                            if (len < 0)
                                len += 256;
                            byte[] ss = new byte[len];
                            buffer.ReadBytes(ss);
                            s = HexUtil.Bytes2HexStr(ss);
                        }
                        break;

                    case TarsStructBase.STRING4:
                        {
                            int len = buffer.ReadInt();
                            if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                                throw new TarsDecodeException("string too long: " + len);
                            byte[] ss = buffer.ReadBytes(len).Array;
                            s = HexUtil.Bytes2HexStr(ss);
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return s;
        }

        public string Read(string s, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.STRING1:
                        {
                            int len = buffer.ReadByte();
                            if (len < 0)
                                len += 256;
                            byte[] ss = new byte[len];
                            buffer.ReadBytes(ss);
                            try
                            {
                                s = sServerEncoding.GetString(ss);
                            }
                            catch
                            {
                                s = Encoding.UTF8.GetString(ss);
                            }
                        }
                        break;
                    case TarsStructBase.STRING4:
                        {
                            int len = buffer.ReadInt();
                            if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                                throw new TarsDecodeException("string too long: " + len);
                            byte[] ss = new byte[len];
                            buffer.ReadBytes(ss);
                            try
                            {
                                s = sServerEncoding.GetString(ss);
                            }
                            catch
                            {
                                s = Encoding.UTF8.GetString(ss);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return s;
        }

        public string ReadString(int tag, bool isRequire)
        {
            string str = null;
            if (SkipToTag(tag))
                str = Read(str, tag, isRequire);
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return str;
        }

        public string[] Read(string[] s, int tag, bool isRequire) => ReadArray(s, tag, isRequire);

        public bool[] Read(bool[] l, int tag, bool isRequire)
        {
            bool[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new bool[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }


        public byte[] Read(byte[] l, int tag, bool isRequire)
        {
            byte[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.SIMPLE_LIST:
                        {
                            var hh = new HeadData();
                            ReadHead(hh);
                            if (hh.Type != TarsStructBase.BYTE)
                                throw new TarsDecodeException("type mismatch, tag: " + tag + ", type: " + hd.Type + ", " + hh.Type);
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("invalid size, tag: " + tag + ", type: " + hd.Type + ", " + hh.Type + ", size: " + size);
                            lr = new byte[size];
                            buffer.ReadBytes(lr);
                            break;
                        }
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new byte[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public short[] Read(short[] l, int tag, bool isRequire)
        {
            short[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new short[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public int[] Read(int[] l, int tag, bool isRequire)
        {
            int[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new int[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public long[] Read(long[] l, int tag, bool isRequire)
        {
            long[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new long[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public float[] Read(float[] l, int tag, bool isRequire)
        {
            float[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new float[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public double[] Read(double[] l, int tag, bool isRequire)
        {
            double[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new double[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = Read(lr[0], 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return lr;
        }

        public IDictionary<string, string> ReadStringMap(int tag, bool isRequire)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            if (SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.MAP:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0) throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                String k = ReadString(0, true);
                                String v = ReadString(1, true);
                                dic.Add(k, v);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return dic;
        }

        public Dictionary<K, V> ReadMap<K, V>(IDictionary<K, V> m, int tag, bool isRequire)
        => (Dictionary<K, V>)ReadMap(new Dictionary<K, V>(), m, tag, isRequire);

        public IDictionary ReadMap(IDictionary m, int tag, bool isRequire)
        {
            Type type = m.GetType();
            Type[] argsType = type.GetGenericArguments();
            var mk = BasicClassTypeUtil.CreateObject(argsType[0]);
            var mv = BasicClassTypeUtil.CreateObject(argsType[1]);
            if (SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.MAP:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0) throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                mk = Read(mk, 0, true);
                                mv = Read(mv, 1, true);
                                if (m.Contains(mk))
                                    m[mk] = mv;
                                else
                                    m.Add(mk, mv);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return m;

        }

        public IDictionary<K, V> ReadMap<K, V>(IDictionary<K, V> mr, IDictionary<K, V> m, int tag, bool isRequire)
        {
            if (m == null || m.Count == 0)
                return new Dictionary<K, V>();
            K mk = default(K);
            V mv = default(V);
            if (SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.MAP:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0) throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                mk = (K)Read(mk, 0, true);
                                mv = (V)Read(mv, 1, true);
                                if (m.ContainsKey(mk))
                                    m[mk] = mv;
                                else
                                    m.Add(mk, mv);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return mr;
        }

        public T[] ReadArray<T>(T[] l, int tag, bool isRequire)
        {
            if (l == null || l.Length == 0)
                throw new TarsDecodeException("unable to get type of key and value.");
            return ReadArrayImpl(l[0], tag, isRequire);
        }
        public IList ReadList<T>(T l, int tag, bool isRequire)
        { 

            IList list = BasicClassTypeUtil.CreateObject(l.GetType()) as IList;
            if (list == null)
            {
                return null;
            }

            object objItem = BasicClassTypeUtil.CreateListItem(list.GetType());

            Array array = ReadArrayImpl(objItem, tag, isRequire);

            if (array != null)
            {
                list.Clear();
                foreach (object obj in array)
                {
                    list.Add(obj);
                }

                return list;
            }

            return null;
        }

        public IList<T> ReadArray<T>(IList<T> l, int tag, bool isRequire)
        {
            if (l == null || l.Count == 0)
            {
                return new List<T>();
            }
            T[] tt = ReadArrayImpl(l[0], tag, isRequire);
            if (tt == null) return null;
            List<T> ll = new List<T>();
            for (int i = 0; i < tt.Length; ++i)
                ll.Add(tt[i]);
            return ll;
        }

        private T[] ReadArrayImpl<T>(T mt, int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = Read(0, 0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            T[] lr = (T[])Array.CreateInstance(mt.GetType(), size);
                            for (int i = 0; i < size; ++i)
                            {
                                T t = (T)Read(mt, 0, true);
                                lr.SetValue(t, i);
                            }
                            return lr;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return null;
        }

        public TarsStructBase DirectRead(TarsStructBase o, int tag, bool isRequire)
        {
            TarsStructBase reff = null;
            if (SkipToTag(tag))
            {
                try
                {
                    reff = o.NewInit();
                }
                catch (Exception e)
                {
                    throw new TarsDecodeException(e.Message);
                }
                var hd = new HeadData();
                ReadHead(hd);
                if (hd.Type != TarsStructBase.STRUCT_BEGIN)
                {
                    throw new TarsDecodeException("type mismatch.");
                }
                reff.ReadFrom(this);
                SkipToStructEnd();
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return reff;
        }

        public TarsStructBase Read(TarsStructBase o, int tag, bool isRequire)
        {
            TarsStructBase reff = null;
            if (SkipToTag(tag))
            {
                try
                {
                    reff = (TarsStructBase)Activator.CreateInstance(o.GetType());
                }
                catch (Exception e)
                {
                    throw new TarsDecodeException(e.Message);
                }

                var hd = new HeadData();
                ReadHead(hd);
                if (hd.Type != TarsStructBase.STRUCT_BEGIN)
                {
                    throw new TarsDecodeException("type mismatch.");
                }
                reff.ReadFrom(this);
                SkipToStructEnd();
            }
            else if (isRequire)
            {
                throw new TarsDecodeException("require field not exist.");
            }
            return reff;
        }

        public TarsStructBase[] Read(TarsStructBase[] o, int tag, bool isRequire)
        {
            return ReadArray(o, tag, isRequire);
        }

        public object Read<T>(T o, int tag, bool isRequire)
        {
            if (o is byte)
            {
                return (Read((byte)0x0, tag, isRequire));
            }
            else if (o is bool)
            {
                return (Read(false, tag, isRequire));
            }
            else if (o is short)
            {
                return Read((short)0, tag, isRequire);
            }
            else if (o is int)
            {
                return Read((int)0, tag, isRequire);
            }
            else if (o is long)
            {
                return (Read((long)0, tag, isRequire));
            }
            else if (o is float)
            {
                return (Read((float)0, tag, isRequire));
            }
            else if (o is double)
            {
                return (Read((double)0, tag, isRequire));
            }
            else if (o is string)
            {
                return (ReadString(tag, isRequire));
            }
            else if (o is IList)
            {
                return ReadList<T>(o, tag, isRequire);
            }
            else if (o is IDictionary)
            {
                IDictionary oo = o as IDictionary;
                return ReadMap(oo, tag, isRequire);
            }
            else if (o is TarsStructBase)
            {
                return Read(o, tag, isRequire);
            }
            else if (o.GetType().IsArray)
            {
                if (o is byte[] || o is Byte[])
                {
                    return Read((byte[])null, tag, isRequire);
                }
                else if (o is bool[])
                {
                    return Read((bool[])null, tag, isRequire);
                }
                else if (o is short[])
                {
                    return Read((short[])null, tag, isRequire);
                }
                else if (o is int[])
                {
                    return Read((int[])null, tag, isRequire);
                }
                else if (o is long[])
                {
                    return Read((long[])null, tag, isRequire);
                }
                else if (o is float[])
                {
                    return Read((float[])null, tag, isRequire);
                }
                else if (o is double[])
                {
                    return Read((double[])null, tag, isRequire);
                }
                else
                {
                    object oo = o;
                    return ReadArray((Object[])oo, tag, isRequire);
                }
            }
            else
            {
                return TarsInputStreamExt.Read(o, tag, isRequire, this);
            }
        }
        public IByteBuffer GetBs()
        {
            return buffer;
        }
    }

}
