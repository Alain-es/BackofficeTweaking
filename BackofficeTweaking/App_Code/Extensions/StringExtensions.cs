using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackofficeTweaking.Extensions
{
    internal static class StringExtensions
    {
        internal static IEnumerable<string> SplitCommaSeparated(this string stringValue)
        {
            return stringValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}