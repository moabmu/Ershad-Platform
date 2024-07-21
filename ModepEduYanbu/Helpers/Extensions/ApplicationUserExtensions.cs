using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static async Task<bool> IsSchoolEmployee(this ApplicationUser user, 
            UserManager<ApplicationUser> userManager)
        {
            var role = (await userManager.GetRolesAsync(user)).ToList()[0];
            return (role == Templates.Roles[3].Name || role == Templates.Roles[4].Name) ? true : false;
        }

        public static string GetCurrentSchoolName(this ApplicationUser user, ISchoolRepo schoolRepo)
        {
            return schoolRepo.GetById(user.CurrentSchoolId).Name;
        }

        public static School GetCurrentSchool(this ApplicationUser user, ISchoolRepo schoolRepo)
        {
            return schoolRepo.GetById(user.CurrentSchoolId);
        }

        /// <summary>
        /// Check if a school user has more than a school.
        /// </summary>
        /// <param name="schoolRepo">Manual injection for School Repository</param>
        /// <returns></returns>
        public static bool HasManySchools(this ApplicationUser user, ISchoolRepo schoolRepo)
        {
            return schoolRepo.GetAllForUser(user.UserName).Count() > 1;
        }
    }
}
