using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs.Support
{
    public class TarsStructInfo
    {
        public List<TarsStructPropertyInfo> PropertyList { set; get; }
        public string Comment { set; get; }
    }
}
