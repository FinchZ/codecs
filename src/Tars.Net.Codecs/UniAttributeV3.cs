using DotNetty.Buffers;
using System;
using System.Collections.Generic;

namespace Tars.Net.Codecs
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
            Temp = convert.Deserialize<IDictionary<string, IByteBuffer>>(buffer, options);
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
            var buf = Unpooled.Buffer(128);
            options.Tag = 0;
            convert.Serialize(obj, type, buf, options);
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