using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs
{
    public class TarsDecodeException : Exception
    {
        public TarsDecodeException(string message)
            : base(message)
        {

        }
    }
}
