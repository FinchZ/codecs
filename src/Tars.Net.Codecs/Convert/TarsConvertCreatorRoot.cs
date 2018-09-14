using System;
using System.Collections.Generic;
using System.Linq;

namespace Tars.Net.Codecs
{
    public class TarsConvertCreatorRoot : ITarsConvertCreatorRoot
    {
        private readonly Dictionary<short, ITarsConvertCreator> creators;

        public TarsConvertCreatorRoot(IEnumerable<ITarsConvertCreator> creators)
        {
            this.creators = creators.ToDictionary(i => i.Version);
        }

        public ITarsStreamConvert Create((Type, short) arg)
        {
            var (type, version) = arg;
            return creators[version].Create(type);
        }
    }
}