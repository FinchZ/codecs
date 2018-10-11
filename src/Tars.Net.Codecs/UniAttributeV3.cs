using DotNetty.Buffers;
using System;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public class UniAttributeV3
    {
        private readonly ITarsConvertRoot convert;
        private readonly ITarsHeadHandler headHandler;

        public IDictionary<string, IByteBuffer> Temp { get; set; }

        public UniAttributeV3(ITarsConvertRoot convert, ITarsHeadHandler headHandler)
        {
            this.convert = convert;
            this.headHandler = headHandler;
        }

        public void Deserialize(IByteBuffer buffer, TarsConvertOptions options)
        {
            var contentBuffer = convert.Deserialize<IByteBuffer>(buffer, options);
            headHandler.ReadHead(contentBuffer, options);
            Temp = convert.Deserialize<IDictionary<string, IByteBuffer>>(contentBuffer, options);
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
            if (obj != null)
            {
                var buf = Unpooled.Buffer(128);
                options.Tag = 0;
                convert.Serialize(obj, type, buf, options);
                if (buf.ReadableBytes == 0) return;
                if (Temp.ContainsKey(name))
                {
                    Temp[name] = buf;
                }
                else
                {
                    Temp.Add(name, buf);
                }
            }
            else if (Temp.ContainsKey(name))
            {
                Temp.Remove(name);
            }
        }
    }
}