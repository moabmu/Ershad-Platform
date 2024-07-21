using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories
{
    public class UsersRepo : IUsersRepo
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public UsersRepo(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetPhoneNumbersForUsersInRolesAsync(string roles)
        {
            var rolesList = roles.Split(',');
            try
            {
                var phones = new List<string>();
                foreach (var role in rolesList)
                {
                    var users = await _userManager.GetUsersInRoleAsync(roles);
                    phones.AddRange(users.Select(x => x.PhoneNumber).ToList());
                }
                return phones;
            }
            catch
            {
                return null;
            }
        }
    }
}
