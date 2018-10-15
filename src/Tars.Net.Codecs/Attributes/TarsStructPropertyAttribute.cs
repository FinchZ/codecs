using System;

namespace Tars.Net.Codecs.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TarsStructPropertyAttribute : Attribute
    {
        public TarsStructPropertyAttribute(int order)
        {
            Order = order;
        }

        public string Comment { get; set; }
        public int Order { get; }
    }
}