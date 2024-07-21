using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IUsersRepo
    {
        Task<List<string>> GetPhoneNumbersForUsersInRolesAsync(string role);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
    }
}
