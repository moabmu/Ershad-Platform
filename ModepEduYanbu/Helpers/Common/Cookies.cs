using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers.Common
{
    public static class Cookies
    {
        public const string EducationalYear = "EducationalYear";
        public const int EducationalYearExpireInDays = 350;

        #region Cookie Helpers
        public static void CreateCookie(HttpContext context, string key, string value, int expireInDays = 350)
        {
            var cookieOptions = new CookieOptions{Expires = DateTime.Now.AddDays(expireInDays)};
            CreateCookie(context, key, value, cookieOptions);
        }

        public static void CreateCookie(HttpContext context, string key, string value, CookieOptions cookieOptions)
        {
            context.Response.Cookies.Append(key, value, cookieOptions);
        }
        #endregion
    }
}