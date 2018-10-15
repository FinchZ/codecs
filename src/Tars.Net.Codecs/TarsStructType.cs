namespace Tars.Net.Codecs
{
    public static class TarsStructType
    {
        public const byte Byte = 0;//8位 整数 0x00
        public const byte Short = 1;//16位 整数 0x01
        public const byte Int = 2;//32位 整数 0x02
        public const byte Long = 3;//64位 整数 0x03
        public const byte Float = 4;//单精度浮点型 0x04
        public const byte Double = 5;//双精度浮点型 0x05
        public const byte String1 = 6;//字符串  0x06 0-255字节
        public const byte String4 = 7;//字符串 0x07  256-4G 不支持4G以上
        public const byte Map = 8;//字典 0x08
        public const byte List = 9;//数组 0x09
        public const byte StructBegin = 10;//结构体 class对象 0x0a
        public const byte StructEnd = 11;//结构体结束符 0x0b
        public const byte Zero = 12;//常数0 0x0c
        public const byte ByteArray = 13;//字节数组 0x0d
    }
}