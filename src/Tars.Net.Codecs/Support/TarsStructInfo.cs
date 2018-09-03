using System;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Codecs.Reflection;

namespace Tars.Net.Codecs.Support
{
    public class TarsStructInfo
    {
        public IConstructorInvoker ConstructorInvoker { set; get; }
        public List<TarsStructPropertyInfo> PropertyList { set; get; }
        public string Comment { set; get; }
    }
}
