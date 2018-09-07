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

        public int GetDataLength() {
            return buffer.ReadInt();
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
                        int size = ReadInt(0, true);
                        for (int i = 0; i < size * 2; ++i)
                        {
                            SkipField();
                        }
                        break;
                    }
                case TarsStructBase.LIST:
                    {
                        int size = ReadInt(0, true);
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
                        int size = ReadInt(0, true);
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
        public string ReadByteString(int tag, bool isRequire)
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
                            return HexUtil.Bytes2HexStr(ss);
                        }
                    case TarsStructBase.STRING4:
                        {
                            int len = buffer.ReadInt();
                            if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                                throw new TarsDecodeException("string too long: " + len);
                            byte[] ss = buffer.ReadBytes(len).Array;
                            return HexUtil.Bytes2HexStr(ss);
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
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
                throw new TarsDecodeException("require field not exist.");
            return reff;
        }
        public object Read(Type type, int tag, bool isRequire)
        {
            if (type.Equals(TarsSupportBaseType.BOOLEN))
                return Convert.ChangeType(ReadByte(tag, isRequire), TarsSupportBaseType.BOOLEN);
            else if (type.Equals(TarsSupportBaseType.BYTE))
                return Convert.ChangeType(ReadByte(tag, isRequire), TarsSupportBaseType.BYTE);
            else if (type.Equals(TarsSupportBaseType.SHORT))
                return Convert.ChangeType(ReadShort(tag, isRequire), TarsSupportBaseType.SHORT);
            else if (type.Equals(TarsSupportBaseType.INT))
                return Convert.ChangeType(ReadInt(tag, isRequire), TarsSupportBaseType.INT);
            else if (type.Equals(TarsSupportBaseType.LONG))
                return Convert.ChangeType(ReadLong(tag, isRequire), TarsSupportBaseType.LONG);
            else if (type.Equals(TarsSupportBaseType.FLOAT))
                return Convert.ChangeType(ReadFloat(tag, isRequire), TarsSupportBaseType.FLOAT);
            else if (type.Equals(TarsSupportBaseType.DOUBLE))
                return Convert.ChangeType(ReadDouble(tag, isRequire), TarsSupportBaseType.DOUBLE);
            else if (type.Equals(TarsSupportBaseType.STRING))
                return Convert.ChangeType(ReadString(tag, isRequire), TarsSupportBaseType.STRING);
            else if (type.IsAssignableFrom(typeof(TarsStructBase)))
                return ReadTarsStructBase(type, tag, isRequire);
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (elementType.Equals(TarsSupportBaseType.BYTE))
                    return ReadByteArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.BOOLEN))
                    return ReadBoolArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.SHORT))
                    return ReadShortArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.INT))
                    return ReadIntArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.LONG))
                    return ReadLongArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.FLOAT))
                    return ReadFloatArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.DOUBLE))
                    return ReadDoubleArray(tag, isRequire);
                if (elementType.Equals(TarsSupportBaseType.STRING))
                    return ReadStringArray(tag, isRequire);
                else if (elementType.IsAssignableFrom(typeof(TarsStructBase)))
                    return ReadTarsStructBaseArray(elementType, tag, isRequire);
                else
                    return Read(elementType, tag, isRequire);
            }
            else if (type.IsGenericType)
            {
                if (type.Equals(TarsSupportBaseType.LIST))
                    return ReadList(type, tag, isRequire);
                else if (type.Equals(TarsSupportBaseType.MAP))
                    return ReadMap(type, tag, isRequire);
                else
                    throw new TarsDecodeException("the class type[" + type.FullName + "] not surpport");
            }
            else
                return TarsInputStreamExt.Read(type, tag, isRequire, this);
        }
        public bool ReadBool(int tag, bool isRequire)
        {
            byte c = ReadByte(tag, isRequire);
            return c != 0;
        }
        public byte ReadByte(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.BYTE: return buffer.ReadByte();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public short ReadShort(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.BYTE:
                        return (short)buffer.ReadByte();
                    case TarsStructBase.SHORT:
                        return buffer.ReadShort();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public int ReadInt(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.BYTE:
                        return (int)buffer.ReadByte();
                    case TarsStructBase.SHORT:
                        return buffer.ReadShort();
                    case TarsStructBase.INT:
                        return buffer.ReadInt();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public long ReadLong(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.BYTE:
                        return (int)buffer.ReadByte();
                    case TarsStructBase.SHORT:
                        return buffer.ReadShort();
                    case TarsStructBase.INT:
                        return buffer.ReadInt();
                    case TarsStructBase.LONG:
                        return buffer.ReadLong();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public float ReadFloat(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.FLOAT:
                        return buffer.ReadFloat();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public double ReadDouble(int tag, bool isRequire)
        {
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.ZERO_TAG:
                        return 0x0;
                    case TarsStructBase.FLOAT:
                        return buffer.ReadFloat();
                    case TarsStructBase.DOUBLE:
                        return buffer.ReadDouble();
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return 0x0;
        }
        public string ReadString(int tag, bool isRequire)
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
                                return sServerEncoding.GetString(ss);
                            }
                            catch
                            {
                                return Encoding.UTF8.GetString(ss);
                            }
                        }
                    case TarsStructBase.STRING4:
                        {
                            int len = buffer.ReadInt();
                            if (len > TarsStructBase.MAX_STRING_LENGTH || len < 0)
                                throw new TarsDecodeException("string too long: " + len);
                            byte[] ss = new byte[len];
                            buffer.ReadBytes(ss);
                            try
                            {
                                return sServerEncoding.GetString(ss);
                            }
                            catch
                            {
                                return Encoding.UTF8.GetString(ss);
                            }
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
        public IList ReadList(Type type, int tag, bool isRequire)
        {
            IList list = (IList)Activator.CreateInstance(type);
            Type[] argsType = type.GetGenericArguments();
            var valueType = argsType[0];
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                var t = Read(valueType, 0, true);
                                list.Add(t);
                            }
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return list;
        }

        internal Dictionary<string, string> ReadMap(int tag, bool isRequire)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.MAP:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0) throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                string mk = ReadString(0, true);
                                string mv = ReadString(1, true);
                                if (dic.ContainsKey(mk))
                                    dic[mk] = mv;
                                else
                                    dic.Add(mk, mv);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return dic;
        }

        public IDictionary ReadMap(Type type, int tag, bool isRequire)
        {
            IDictionary dic = (IDictionary)Activator.CreateInstance(type);
            Type[] argsType = type.GetGenericArguments();
            var keyType = argsType[0];
            var valueType = argsType[1]; ;
            if (SkipToTag(tag))
            {
                HeadData hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.MAP:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0) throw new TarsDecodeException("size invalid: " + size);
                            for (int i = 0; i < size; ++i)
                            {
                                var mk = Read(keyType, 0, true);
                                var mv = Read(valueType, 1, true);
                                if (dic.Contains(mk))
                                    dic[mk] = mv;
                                else
                                    dic.Add(mk, mv);
                            }
                        }
                        break;
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return dic;
        }
        public bool[] ReadBoolArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new bool[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadBool(0, true);
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
        public byte[] ReadByteArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("invalid size, tag: " + tag + ", type: " + hd.Type + ", " + hh.Type + ", size: " + size);
                            lr = new byte[size];
                            buffer.ReadBytes(lr);
                            break;
                        }
                    case TarsStructBase.LIST:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new byte[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadByte(0, true);
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
        public short[] ReadShortArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new short[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadShort(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public int[] ReadIntArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new int[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadInt(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public long[] ReadLongArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new long[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadLong(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public float[] ReadFloatArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new float[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadFloat(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public double[] ReadDoubleArray(int tag, bool isRequire)
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
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new double[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadDouble(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public string[] ReadStringArray(int tag, bool isRequire)
        {
            string[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new string[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadString(0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }
        public TarsStructBase ReadTarsStructBase(Type type, int tag, bool isRequire)
        {
            TarsStructBase reff = null;
            if (SkipToTag(tag))
            {
                try
                {
                    reff = (TarsStructBase)Activator.CreateInstance(type);
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
        private TarsStructBase[] ReadTarsStructBaseArray(Type elementType, int tag, bool isRequire)
        {
            TarsStructBase[] lr = null;
            if (SkipToTag(tag))
            {
                var hd = new HeadData();
                ReadHead(hd);
                switch (hd.Type)
                {
                    case TarsStructBase.LIST:
                        {
                            int size = ReadInt(0, true);
                            if (size < 0)
                                throw new TarsDecodeException("size invalid: " + size);
                            lr = new TarsStructBase[size];
                            for (int i = 0; i < size; ++i)
                                lr[i] = ReadTarsStructBase(elementType, 0, true);
                            break;
                        }
                    default:
                        throw new TarsDecodeException("type mismatch.");
                }
            }
            else if (isRequire)
                throw new TarsDecodeException("require field not exist.");
            return lr;
        }

        public IByteBuffer GetBs()
        {
            return buffer;
        }
    }

}
