using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs
{
    public class TarsEncodeException : Exception
    {
        public TarsEncodeException(string message) 
            : base(message)
        {
        }
    }
}
