using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TarsStructPropertyAttribute : Attribute
    {
        public int Order { get; set; }
        public bool IsRequire { get; set; }
        public string Comment { get; set; }
    }
}
