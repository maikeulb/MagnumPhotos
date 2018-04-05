using System.Collections.Generic;

namespace MagnumPhotos.API.Models
{
    public class LinkedCollectionResourceWrapper<T> : LinkedResourceBase
    where T : LinkedResourceBase
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapper (IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
