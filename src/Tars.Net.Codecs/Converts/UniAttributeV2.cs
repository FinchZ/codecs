using DotNetty.Buffers;
using System;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public class UniAttributeV2
    {
        public IDictionary<string, IDictionary<string, IByteBuffer>> Temp { get; set; }

        private readonly ITarsConvertRoot convert;

        public UniAttributeV2(ITarsConvertRoot convert)
        {
            this.convert = convert;
        }

        public void Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = convert.Deserialize<IByteBuffer>(buffer, options);
            Temp = convert.Deserialize<IDictionary<string, IDictionary<string, IByteBuffer>>>(buf, options);
        }

        public void Serialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var buf = Unpooled.Buffer(128);
            var oldTag = options.Tag;
            options.Tag = 0;
            convert.Serialize(Temp, buf, options);
            options.Tag = oldTag;
            convert.Serialize(buf, buffer, options);
        }

        public void Put(string name, object obj, Type type, TarsConvertOptions options)
        {
            if (name == null)
            {
                throw new ArgumentException("put key can not be null");
            }

            if (obj == null)
            {
                throw new ArgumentException("put value can not be null");
            }

            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(obj, buf, options);
            Dictionary<string, IByteBuffer> pair = new Dictionary<string, IByteBuffer>(1) { { string.Empty, buf } };
            if (Temp.ContainsKey(name))
            {
                Temp[name] = pair;
            }
            else
            {
                Temp.Add(name, pair);
            }
        }
    }
}