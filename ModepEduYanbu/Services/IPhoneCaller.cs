using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Services
{
    public interface IPhoneCaller
    {
        Task<IActionResult> MakePhoneCallAsync(string number, string message, HttpContext context);
    }
}
