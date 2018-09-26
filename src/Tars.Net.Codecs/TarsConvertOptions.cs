using System.Text;

namespace Tars.Net.Codecs
{
    public class TarsConvertOptions
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public short Version { get; set; } = TarsCodecsVersion.V3;

        public Codec Codec { get; set; } = Codec.Tars;

        public bool HasValue { get; set; } = true;

        public int Tag { get; set; } = 1;

        public byte TarsType { get; set; }

        public override string ToString()
        {
            return $"TarsType:{TarsType},Codec:{Codec},Version:{Version},Tag:{Tag},Encoding:{Encoding}";
        }
    }
}