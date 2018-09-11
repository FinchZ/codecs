using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs.Exceptions
{
    //todo  move
 
    public class ProtocolException : Exception
    {
        public ProtocolException(string message)
            : base(message)
        {

        }
    }
}
