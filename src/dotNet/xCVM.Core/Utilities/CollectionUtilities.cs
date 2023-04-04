using System;
using System.Collections.Generic;
using System.Text;

namespace xCVM.Core.Utilities
{
    public static class CollectionUtilities
    {
        public static void Concatenate<T>(this List<T> HEAD, IEnumerable<T> TAIL)
        {
            foreach (var item in TAIL)
            {
                HEAD.Add(item);
            }
        }
    }
}
