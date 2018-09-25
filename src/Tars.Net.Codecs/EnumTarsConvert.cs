using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class EnumTarsConvert<T> : TarsConvertBase<T> where T : struct, IConvertible, IComparable, IFormattable
    {
        private readonly Type realType;
        private readonly ITarsConvertRoot convertRoot;

        public EnumTarsConvert(ITarsConvertRoot convertRoot)
        {
            realType = Enum.GetUnderlyingType(typeof(T));
            this.convertRoot = convertRoot;
        }

        public override T Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            return (T)convertRoot.Deserialize(buffer, realType, options);
        }

        public override void Serialize(T obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convertRoot.Serialize(obj, realType, buffer, options);
        }
    }
}