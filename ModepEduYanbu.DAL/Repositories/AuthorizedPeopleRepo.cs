using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModepEduYanbu.Models;
using ModepEduYanbu.Data;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Repositories
{
    public class AuthorizedPeopleRepo : IAuthorizedPeopleRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizedPeopleRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public AuthorizedPerson Add(AuthorizedPerson entity)
        {
            if (!this.CanAdd(entity).Result)
                return null;

            _context.AuthorizedPeople.Add(entity);
            return entity;
        }

        public AuthorizedPerson Remove(AuthorizedPerson entity)
        {
            _context.AuthorizedPeople.Remove(entity);
            return entity;
        }

        public IEnumerable<AuthorizedPerson> GetAll()
        {
            return _context.AuthorizedPeople
                .Include(x => x.AddedByUser)
                .Include(x => x.Role)
                .ToList();
            //return (from p in _context.AuthorizedPeople select p);
        }

        public AuthorizedPerson GetById(string id)
        {
            return _context.AuthorizedPeople
                .Include(x => x.AddedByUser)
                .Include(x => x.Role)
                .FirstOrDefault(p => p.AuthorizedPersonId == id);
        }

        public AuthorizedPerson GetByIdNo(string idNo)
        {
            var result = _context.AuthorizedPeople
                .Include(x => x.AddedByUser)
                .Include(x => x.Role)
                .FirstOrDefault(p => p.IdNo == idNo);
            return result;
        }

        public IQueryable<AuthorizedPerson> GetBySchoolMinistryNo(string schoolMinistryNo)
        {
            return _context.AuthorizedPeople
                .Include(x => x.AddedByUser)
                .Include(x => x.Role)
                .Where(p => p.SchoolMinistryNo == schoolMinistryNo);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public bool IsIdNoValid(string idNo)
        {
            return _context.AuthorizedPeople.Any(x => x.IdNo == idNo);
        }

        public bool PersonMatchesSchoolMinistryNo(string personIdNo, string schoolMinistryNo)
        {
            return _context.AuthorizedPeople.Any(x => x.IdNo == personIdNo && x.SchoolMinistryNo == schoolMinistryNo);
        }

        public bool IsEduDepEmployee(string idNo)
        {
            return _context.AuthorizedPeople.Any(p => p.IdNo == idNo
            && (p.SchoolMinistryNo == null || p.SchoolMinistryNo == "")
            );
        }

        public void RemoveAllRecordsByIdNo(string idNo)
        {
            var allRecords = _context.AuthorizedPeople.Where(x => x.IdNo == idNo);
            foreach(var record in allRecords)
            {
                this.Remove(record);
            }
        }

        public IEnumerable<AuthorizedPerson> GetAllRecordsByIdNo(string idNo)
        {
            return _context.AuthorizedPeople
                .Include(x => x.Role)
                .Where(x => x.IdNo == idNo)
                .AsEnumerable();
        }

        public async Task<bool> CanAdd(AuthorizedPerson entity)
        {
            if (entity == null)
                return false;

            if (_context.AuthorizedPeople.Where(x => x.IdNo == entity.IdNo && x.SchoolMinistryNo == entity.SchoolMinistryNo).FirstOrDefault() != null)
                return false;

            var isPersonAlreadyRegistered = (await _userManager.FindByEmailAsync(entity.IdNo)) != null;
            if (isPersonAlreadyRegistered)
                return false;

            var allRecords = this.GetAllRecordsByIdNo(entity.IdNo);
            if (allRecords.Any(x => x.Role != entity.Role)) // if the assigned role is different to the role in the past records
                return false;

            return true;
        }

        public async Task<bool> CanAdd(string idNo)
        {
            var person = this.GetByIdNo(idNo);
            return await this.CanAdd(person);
        }
    }
}
