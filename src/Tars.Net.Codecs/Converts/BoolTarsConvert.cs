using DotNetty.Buffers;
using System;

namespace Tars.Net.Codecs
{
    public class BoolTarsConvert : TarsConvertBase<bool>
    {
        public BoolTarsConvert(IServiceProvider provider) : base(provider)
        {
        }

        public override bool DeserializeT(IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            byte c = convertRoot.Deserialize<byte>(buffer, order, isRequire, options);
            return c != 0;
        }

        public override void SerializeT(bool obj, IByteBuffer buffer, int order, bool isRequire, TarsConvertOptions options)
        {
            convertRoot.Serialize((byte)(obj ? 0x01 : 0), order, isRequire, options);
        }
    }
}