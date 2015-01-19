using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackofficeTweaking.Extensions
{
    internal static class IListExtensions
    {
        public static void AddRangeUnique<T>(this IList<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!self.Contains(item))
                {
                    self.Add(item);
                }
            }
        }
    }

}