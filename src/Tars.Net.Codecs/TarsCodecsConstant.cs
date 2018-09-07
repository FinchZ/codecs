using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Codecs
{
   public static  class TarsCodecsConstant
    {
        public const byte NORMAL = 0x00; 
        public const byte ONEWAY = 0x01;
        public const int PACKAGE_MAX_LENGTH = 10 * 1024 * 1024;
        public const int HEAD_SIZE = 4;

        public const short VERSION = 0x01; //tars

        public const short VERSION2 = 0x02;//TUP1

        public const short VERSION3 = 0x03;//TUP2

        public const int DEFAULT_TICKET_NUMBER =-1;
    }
}
