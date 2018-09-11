using DotNetty.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Codecs.Util;

namespace Tars.Net.Codecs
{

    public class TarsOutputStream
    {
        private IByteBuffer buf;

        public TarsOutputStream() : this(128) { }
        public TarsOutputStream(int capacity)
        {
            buf = Unpooled.Buffer(capacity);
        }

        public TarsOutputStream(IByteBuffer buf)
        {
            this.buf = buf;
        }

        public byte[] ToByteArray()
        {
            byte[] newBytes = new byte[buf.WriterIndex];
            buf.SetReaderIndex(0);
            buf.ReadBytes(newBytes, 0, newBytes.Length);
            buf.SetReaderIndex(0);
            return newBytes;
        }

        public void Reserve(int len)
        {
            buf.EnsureWritable(len, true);
        }

        public void ResetDataLength(int length)
        {
            buf.SetInt(0, length);
        }

        public IByteBuffer GetByteBuffer() => buf;

        // 写入头信息
        public void WriteHead(byte type, int tag)
        {
            if (tag < 15)
            {
                byte b = (byte)((tag << 4) | type);
                buf.WriteByte(b);
            }
            else if (tag < 256)
            {
                byte b = (byte)((15 << 4) | type);
                buf.WriteByte(b);
                buf.WriteByte(tag);
            }
            else
            {
                throw new TarsEncodeException("tag is too large: " + tag);
            }
        }

        public void Write(bool b, int tag)
        {
            byte by = (byte)(b ? 0x01 : 0);
            Write(by, tag);
        }

        public void Write(byte b, int tag)
        {
            Reserve(3);
            if (b == 0)
            {
                WriteHead(TarsStructBase.ZERO_TAG, tag);
            }
            else
            {
                WriteHead(TarsStructBase.BYTE, tag);
                buf.WriteByte(b);
            }
        }

        public void Write(short n, int tag)
        {
            Reserve(4);
            if (n >= -128 && n <= 127)
            {
                Write((byte)n, tag);
            }
            else
            {
                WriteHead(TarsStructBase.SHORT, tag);
                buf.WriteShort(n);
            }
        }

        public void Write(int n, int tag)
        {
            Reserve(6);
            if (n >= short.MinValue && n <= short.MaxValue)
            {
                Write((short)n, tag);
            }
            else
            {
                WriteHead(TarsStructBase.INT, tag);
                buf.WriteInt(n);
            }
        }

        public void Write(long n, int tag)
        {
            Reserve(10);
            if (n >= int.MinValue && n <= int.MaxValue)
            {
                Write((int)n, tag);
            }
            else
            {
                WriteHead(TarsStructBase.LONG, tag);
                buf.WriteLong(n);
            }
        }

        public void Write(float n, int tag)
        {
            Reserve(6);
            WriteHead(TarsStructBase.FLOAT, tag);
            buf.WriteFloat(n);
        }

        public void Write(double n, int tag)
        {
            Reserve(10);
            WriteHead(TarsStructBase.DOUBLE, tag);
            buf.WriteDouble(n);
        }

        public void WriteStringByte(string s, int tag)
        {
            byte[] by = HexUtil.HexStr2Bytes(s);
            Reserve(10 + by.Length);
            if (by.Length > 255)
            {
                // 长度大于255，为String4类型
                WriteHead(TarsStructBase.STRING4, tag);
                buf.WriteInt(by.Length);
                buf.WriteBytes(by);
            }
            else
            {
                // 长度小于255，位String1类型
                WriteHead(TarsStructBase.STRING1, tag);
                buf.WriteByte(by.Length);
                buf.WriteBytes(by);
            }
        }

        public void WriteByteString(string s, int tag)
        {
            Reserve(10 + s.Length);
            byte[] by = HexUtil.HexStr2Bytes(s);
            if (by.Length > 255)
            {
                WriteHead(TarsStructBase.STRING4, tag);
                buf.WriteInt(by.Length);
                buf.WriteBytes(by);
            }
            else
            {
                WriteHead(TarsStructBase.STRING1, tag);
                buf.WriteByte(by.Length);
                buf.WriteBytes(by);
            }
        }

        public void Write(string s, int tag)
        {
            byte[] by;
            by = Encoding.GetEncoding(sServerEncoding).GetBytes(s);
            Reserve(10 + by.Length);
            if (by.Length > 255)
            {
                WriteHead(TarsStructBase.STRING4, tag);
                buf.WriteInt(by.Length);
                buf.WriteBytes(by);
            }
            else
            {
                WriteHead(TarsStructBase.STRING1, tag);
                buf.WriteByte(by.Length);
                buf.WriteBytes(by);
            }
        }


        public void Write(bool[] l, int tag)
        {

            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(l.Length, 0);
            foreach (var e in l)
            {
                Write(e, 0);
            }

        }

        public void Write(byte[] l, int tag)
        {
            int len = l.Length;
            Reserve(8 + len);
            WriteHead(TarsStructBase.SIMPLE_LIST, tag);
            WriteHead(TarsStructBase.BYTE, 0);
            Write(len, 0);
            buf.WriteBytes(l);
        }

        public void Write(short[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (short e in l)
            {
                Write(e, 0);
            }
        }

        public void Write(int[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (int e in l)
            {
                Write(e, 0);
            }
        }

        public void Write(long[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (long e in l)
            {
                Write(e, 0);
            }
        }

        public void Write(float[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (float e in l)
            {
                Write(e, 0);
            }
        }

        public void Write(double[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (double e in l)
            {
                Write(e, 0);
            }
        }
        private void WriteArray(object[] l, int tag)
        {
            int nLen = l.Length;
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(nLen, 0);
            foreach (Object e in l)
                Write(e, 0);
        }
        public void WriteList(IList l, int tag)
        {
            Reserve(8);
            WriteHead(TarsStructBase.LIST, tag);
            Write(l == null ? 0 : (l.Count > 0 ? l.Count : 0), 0);
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    Write(l[i], 0);
                }
            }
        }

        public void Write(IDictionary<string, string> m, int tag)
        {
            Reserve(8);
            WriteHead(TarsStructBase.MAP, tag);
            Write(m == null ? 0 : m.Count, 0);
            if (m != null)
            {
                foreach (KeyValuePair<string, string> en in m)
                {
                    Write(en.Key, 0);
                    Write(en.Value, 1);
                }
            }
        }

        public void Write<K, V>(IDictionary<K, V> m, int tag)
        {
            Reserve(8);
            WriteHead(TarsStructBase.MAP, tag);
            Write(m == null ? 0 : m.Count, 0);
            if (m != null)
            {
                foreach (KeyValuePair<K, V> en in m)
                {
                    Write(en.Key, 0);
                    Write(en.Value, 1);
                }
            }
        }



        public void Write(IDictionary m, int tag)
        {
            Reserve(8);
            WriteHead(TarsStructBase.MAP, tag);
            Write(m == null ? 0 : m.Count, 0);
            if (m != null)
            {
                ICollection keys = m.Keys;
                foreach (object objKey in keys)
                {
                    Write(objKey, 0);
                    Write(m[objKey], 1);
                }
            }
        }

        public void Write(TarsStructBase o, int tag)
        {
            Reserve(2);
            WriteHead(TarsStructBase.STRUCT_BEGIN, tag);
            o.WriteTo(this);
            Reserve(2);
            WriteHead(TarsStructBase.STRUCT_END, 0);
        }

        public void Write(object o, int tag)
        {
            if (o is byte)
            {
                Write(((byte)o), tag);
            }
            else if (o is bool)
            {
                Write((bool)o, tag);
            }
            else if (o is short)
            {
                Write(((short)o), tag);
            }
            else if (o is int)
            {
                Write(((int)o), tag);
            }
            else if (o is long)
            {
                Write(((long)o), tag);
            }
            else if (o is float)
            {
                Write(((float)o), tag);
            }
            else if (o is double)
            {
                Write(((double)o), tag);
            }
            else if (o is string)
            {
                Write((string)o, tag);
            }
            else if (o is byte[])
            {
                Write((byte[])o, tag);
            }
            else if (o is bool[])
            {
                Write((bool[])o, tag);
            }
            else if (o is short[])
            {
                Write((short[])o, tag);
            }
            else if (o is int[])
            {
                Write((int[])o, tag);
            }
            else if (o is long[])
            {
                Write((long[])o, tag);
            }
            else if (o is float[])
            {
                Write((float[])o, tag);
            }
            else if (o is double[])
            {
                Write((double[])o, tag);
            }
            else if (o is TarsStructBase)
            {
                Write((TarsStructBase)o, tag);
            }
            else if (o.GetType().IsArray)
            {
                WriteArray((object[])o, tag);
            }
            else if (o is IList)
            {
                WriteList((IList)o, tag);
            }
            else if (o is IDictionary)
            {
                Write((IDictionary)o, tag);
            }
            else
            {
                TarsOutputStreamExt.Write(o, tag, this);
            }
        }
        protected string sServerEncoding = "UTF-8";

        public int SetServerEncoding(string se)
        {
            sServerEncoding = se;
            return 0;
        }
    }
}
