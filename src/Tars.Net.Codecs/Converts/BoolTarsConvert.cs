using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class BoolTarsConvert : TarsConvertBase<bool>
    {
        public BoolTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override bool Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var value = convertRoot.Deserialize<byte>(buffer, options);
            return value != 0;
        }

        public override void Serialize(bool obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            convertRoot.Serialize((byte)(obj ? 0x01 : 0), buffer, options);
        }
    }
}