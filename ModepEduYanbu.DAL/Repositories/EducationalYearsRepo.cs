using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories
{
    public class EducationalYearsRepo : IEducationalYearsRepo
    {
        private readonly ApplicationDbContext _context;

        public EducationalYearsRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public EducationalYear Add(EducationalYear entity)
        {
            _context.EducationalYears.Add(entity);
            return entity;
        }

        public IEnumerable<EducationalYear> GetAll(bool includeEduPrograms = false)
        {
            if (_context.EducationalYears.Count() < 1)
                this.GetCurrentYearOrCreateIfNotExist();

            IEnumerable<EducationalYear> result = null;
            if (includeEduPrograms)
                result = _context.EducationalYears.Include(x => x.EduPrograms).AsEnumerable();
            else
                result = _context.EducationalYears.AsEnumerable();
            return result.OrderByDescending(x => x.EndDate);
        }

        public IEnumerable<EducationalYear> GetAll()
        {
            return this.GetAll(false);
        }

        public EducationalYear GetById(string id, bool includeEduPrograms = false)
        {
            EducationalYear result = null;
            if (includeEduPrograms)
                result = _context.EducationalYears
                    .Include(x => x.EduPrograms)
                    .FirstOrDefault(x => x.EducationalYearId == id);
            else
                result = _context.EducationalYears.FirstOrDefault(x => x.EducationalYearId == id);
            return result;
        }

        public EducationalYear GetById(string id)
        {
            return this.GetById(id, false);
        }

        public EducationalYear GetCurrentYearOrCreateIfNotExist(bool includeEduPrograms = false)
        {
            var currentDate = DateTime.Now;
            EducationalYear year = null;
            Expression<Func<EducationalYear, bool>> filter = x => x.BeginDate <= currentDate && x.EndDate >= currentDate;
            if (includeEduPrograms)
                year = _context.EducationalYears
                    .Include(x => x.EduPrograms)
                    .FirstOrDefault(filter);
            else
                year = _context.EducationalYears.FirstOrDefault(filter);

            if (year == null)
            {
                year = new EducationalYear
                {
                    BeginDate = new DateTime(currentDate.Year, 8, 15),
                    EndDate = new DateTime(currentDate.Year + 1, 8, 14, 23, 59, 59),
                    Name = $"العام الدراسي {currentDate.Year} - {currentDate.Year + 1}",
                    ShortName = $"{currentDate.Year} - {currentDate.Year + 1}"
                };
                _context.EducationalYears.Add(year);
                _context.SaveChanges();
            }
            return year;
        }

        public EducationalYear Remove(EducationalYear entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
