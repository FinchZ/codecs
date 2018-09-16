using System.Text;

namespace Tars.Net.Codecs
{
    public class TarsConvertOptions
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public short Version { get; set; } = TarsCodecsConstant.VERSION3;

        public Codec Codec { get; set; } = Codec.Tars;

        public bool HasValue { get; set; } = true;
        public int Tag { get; internal set; }
        public byte TarsType { get; internal set; }
    }
}