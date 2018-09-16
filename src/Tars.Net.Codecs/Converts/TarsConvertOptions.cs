using System.Text;

namespace Tars.Net.Codecs
{
    public class TarsConvertOptions
    {
        public static readonly TarsConvertOptions Default = new TarsConvertOptions()
        {
            Encoding = Encoding.UTF8,
            Version = TarsCodecsConstant.VERSION3
        };

        public Encoding Encoding { get; set; }

        public short Version { get; set; }
    }
}