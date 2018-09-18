using DotNetty.Buffers;
using System.Collections.Generic;

namespace Tars.Net.Codecs.Converts
{
    public class UniAttributeV3
    {
        private readonly ITarsConvertRoot convert;
        public IDictionary<string, IByteBuffer> Temp { get; set; }

        public UniAttributeV3(ITarsConvertRoot convert)
        {
            this.convert = convert;
        }

        public void Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = convert.Deserialize<IByteBuffer>(buffer, options);
            Temp = convert.Deserialize<IDictionary<string, IByteBuffer>>(buf, options);
        }

        public void Serialize(IDictionary<string, IByteBuffer> obj, IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(obj, buf, options);
            convert.Serialize(buf, buffer, options);
        }

        public void Put<T>(string name, T obj, TarsConvertOptions options)
        {
            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(obj, buf, options);
            if (Temp.ContainsKey(name))
            {
                Temp[name] = buf;
            }
            else
            {
                Temp.Add(name, buf);
            }
        }
    }
}