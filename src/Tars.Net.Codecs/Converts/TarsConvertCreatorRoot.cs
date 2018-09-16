using System;
using System.Collections.Generic;
using System.Linq;

namespace Tars.Net.Codecs
{
    public class TarsConvertCreatorRoot : ITarsConvertCreatorRoot
    {
        private readonly ITarsConvertCreator[] creators;

        public TarsConvertCreatorRoot(IEnumerable<ITarsConvertCreator> creators)
        {
            this.creators = creators.OrderBy(i => i.Order).ToArray();
        }

        public ITarsStreamConvert Create((Type, short) options)
        {
            var creator = creators.FirstOrDefault(i => i.Accept(options));
            var convert = creator?.Create(options);
            if (convert == null)
            {
                throw new NotSupportedException($"Codecs not supported {options}.");
            }
            return convert;
        }
    }
}