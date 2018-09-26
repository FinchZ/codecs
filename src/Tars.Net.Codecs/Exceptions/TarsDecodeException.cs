using System;

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