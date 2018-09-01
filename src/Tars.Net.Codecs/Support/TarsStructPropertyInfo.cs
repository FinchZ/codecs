using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs.Support
{
    public class TarsStructPropertyInfo
    {
        public bool IsRequire { set; get; }
        public int Order { set; get; }
        public Object DefaultValue { set; get; }
        public String Name { set; get; }
        public Object Stamp { set; get; }
        public String Comment { set; get; }
    }
}
