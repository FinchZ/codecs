using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tars.Net.Codecs.Reflection;

namespace Tars.Net.Codecs.Support
{
    public class TarsStructPropertyInfo
    {
        public bool IsRequire { set; get; }
        public int Order { set; get; } 
        public String Name { set; get; } 
        public String Comment { set; get; }
        public IPropertyAccessor PropertyAccessor { set; get; }
        public Type Type { set; get; }
    }
}
