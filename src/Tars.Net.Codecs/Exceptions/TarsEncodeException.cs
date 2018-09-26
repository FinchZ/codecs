using System;

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