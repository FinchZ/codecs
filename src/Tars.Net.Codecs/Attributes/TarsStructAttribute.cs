using System;

namespace Tars.Net.Codecs.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TarsStructAttribute : Attribute
    {
        public string Comment { get; set; }
    }
}