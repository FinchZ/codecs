using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs
{
    //tars支持的数据类型
    public abstract class TarsStructBase
    { 
        public const byte BYTE = 0;//8位 整数 0x00 
        public const byte SHORT = 1;//16位 整数 0x01
        public const byte INT = 2;//32位 整数 0x02
        public const byte LONG = 3;//64位 整数 0x03
        public const byte FLOAT = 4;//单精度浮点型 0x04
        public const byte DOUBLE = 5;//双精度浮点型 0x05
        public const byte STRING1 = 6;//字符串  0x06 0-255字节
        public const byte STRING4 = 7;//字符串 0x07  256-4G 不支持4G以上
        public const byte MAP = 8;//字典 0x08
        public const byte LIST = 9;//数组 0x09
        public const byte STRUCT_BEGIN = 10;//结构体 class对象 0x0a
        public const byte STRUCT_END = 11;//结构体结束符 0x0b
        public const byte ZERO_TAG = 12;//常数0 0x0c
        public const byte SIMPLE_LIST = 13;//字节数组 0x0d  

        public const int MAX_STRING_LENGTH = 100 * 1024 * 1024;

        public TarsStructBase NewInit()
        {
            return null;
        }

        public abstract void ReadFrom(TarsInputStream stream);

        internal void WriteTo(TarsOutputStream tarsOutputStream)
        {
            throw new NotImplementedException();
        }
    }
}
