using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModepEduYanbu.Models;
using ModepEduYanbu.Data;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.Repositories.Interfaces;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Repositories
{
    public class SchoolRepo : ISchoolRepo
    {
        private readonly ApplicationDbContext _context;

        public SchoolRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public School Add(School entity)
        {
            _context.Schools.Add(entity);
            return entity;
        }

        public IEnumerable<School> GetAll()
        {
            var result = _context.Schools//.Include(s => s.Level)
                .ToList();
            return result;
        }

        public School GetById(string id)
        {
            return _context.Schools
                .Include(x => x.Level)
                .FirstOrDefault(x => x.SchoolId == id);
        }

        public School GetByMinistryNo(string ministryNo, bool includeUsersForSchool = false)
        {
            School result;
            if (includeUsersForSchool)
            {
                result = _context.Schools
                    .Include(x => x.Level)
                    .Include(x => x.Type)
                    .Include(x => x.UsersForSchool) // Include users to a school
                    .FirstOrDefault(x => x.MinistryNo == ministryNo);
            }
            else
            {
                result = _context.Schools
                    .Include(x => x.Level)
                    .Include(x => x.Type)
                    .FirstOrDefault(x => x.MinistryNo == ministryNo);
            }
            return result;
        }

        public IQueryable<School> GetAllByAuthorizedPersonIdNo(string authorizedPersonIdNo, bool includeUsersForSchool = false)
        {
            var allDataForAuthorizedPerson = _context.AuthorizedPeople
                .Where(p => p.IdNo == authorizedPersonIdNo).ToList();

            if (includeUsersForSchool)
            {
                return _context.Schools
                    .Include(x => x.Level)
                    .Include(x => x.Type)
                    .Include(x => x.UsersForSchool)
                    .Where(school => allDataForAuthorizedPerson.Any(p => p.SchoolMinistryNo == school.MinistryNo));
            }
            else
            {
                return _context.Schools
                    .Include(x => x.Level)
                    .Include(x => x.Type)
                    .Where(school => allDataForAuthorizedPerson.Any(p => p.SchoolMinistryNo == school.MinistryNo));
            }
        }

        /// <summary>
        /// Get schools by user name.
        /// </summary>
        /// <param name="username">username is the IdNo.</param>
        /// <param name="includeUsersForSchool"></param>
        /// <returns></returns>
        public List<School> GetAllForUser(string username, bool includeUsersForSchool = false)
        {
            var userSchools = _context.Users.Include(x => x.UserSchools).Where(x => x.UserName == username).FirstOrDefault()?.UserSchools;
            var schools = new List<School>();
            userSchools.ForEach(x => schools.Add(_context.Schools.Where(s => s.SchoolId == x.SchooId).FirstOrDefault()));
            return schools;

        }

        public School UpdatePrincipleIdNo(School school,string newPrincipleId)
        {
            school.PrincipleIdNo = newPrincipleId;
            _context.Attach(school);
            _context.Entry(school).Property(x => x.PrincipleIdNo).IsModified = true;
            return school;
        }

        public School UpdatePrincipleIdNo(string schoolId, string newPrincipleId)
        {
            return this.UpdatePrincipleIdNo(_context.Schools.FirstOrDefault(x => x.SchoolId == schoolId), newPrincipleId);
        }

        public bool IsSchoolMinistryNoValid(string ministryNo)
        {
            return _context.Schools.Any(s => s.MinistryNo == ministryNo);
        }

        public School Remove(School entity)
        {
            _context.Schools.Remove(entity);
            return entity;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public bool SchoolIdMatchesMinistryNo(string schoolId, string schoolMinistryNo)
        {
            return _context.Schools
                .Any(s => s.SchoolId == schoolId && s.MinistryNo == schoolMinistryNo);
        }

        public ApplicationUser GetPrinciple(string schoolId)
        {
            return this.GetPrinciple(_context.Schools.FirstOrDefault(x => x.SchoolId == schoolId));
        }

        public ApplicationUser GetPrinciple(School school)
        {
            return _context.Users.FirstOrDefault(x => x.UserName == school.PrincipleIdNo);
        }
    }
}
