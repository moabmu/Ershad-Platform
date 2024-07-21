using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string GetFirstNameAr(this string value)
        {
            var fullname = value.Trim();
            try
            {
                return fullname.Substring(0, fullname.IndexOf(' ') + 1);
            }
            catch
            {
                return fullname;
            }
        }
    }
}
