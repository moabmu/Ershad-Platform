using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
        Task SendSmsAsync(IEnumerable<string> numbers, string message);
    }
}
