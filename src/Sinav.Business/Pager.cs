using System.Collections;
using System.Collections.Generic;

namespace Sinav.Business
{
    public class Pager<T> where T :class
    {
        public IEnumerable<T>  Items { get; set; }
        public int Count;
    }
}