using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs
{
    public class HeadData
    {
        public byte Type { set; get; }
        public int Tag { set; get; }
        public void Clear()
        {
            Type = 0;
            Tag = 0;
        }
    }
}
