using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class BoolTarsConvert : TarsConvertBase<bool>
    {
        public BoolTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override (int order, bool value) Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var (order, value) = convertRoot.Deserialize<byte>(buffer, options);
            return (order, value != 0);
        }

        public override void Serialize(bool obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            convertRoot.Serialize((byte)(obj ? 0x01 : 0), buffer, order, isRequire, options);
        }
    }
}