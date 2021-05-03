using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HplusSport.API.Classes
{
    public class QueryParameters
    {
        // Maximum size is 100
        const int _maxSize = 100;

        // This is my default size
        private int _size = 50;

        public int Page { get; set; }
        public int Size
        {
            get
            {
                return _size;

            }

            // This is for when the user provides a size.  We allow maximum 100
            set
            {
                _size = Math.Min(_maxSize, value);
            }
        }
    }
}
