using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModepEduYanbu.Models;
using ModepEduYanbu.Data;
using ModepEduYanbu.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModepEduYanbu.DAL.DbContexts;

namespace ModepEduYanbu.Repositories
{
    public class EduProgramsRepo : IEduProgramsRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IEducationalYearsRepo _educationalYearsRepo;

        public EduProgramsRepo(ApplicationDbContext context, IEducationalYearsRepo educationalYearsRepo)
        {
            _context = context;
            _educationalYearsRepo = educationalYearsRepo;
        }
        public EduProgram Add(EduProgram entity)
        {
            entity.EducationalYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            _context.EduPrograms.Add(entity);
            return entity;
        }

        public IEnumerable<EduProgram> GetAll()
        {
            var currentYear = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            return _context.EduPrograms.Where(x => x.EducationalYearId == currentYear.EducationalYearId).AsEnumerable();
        }

        public EduProgram GetById(string id)
        {
            return _context.EduPrograms.Include(x => x.DescriptionFile).FirstOrDefault(p => p.EduProgramId == id);
        }

        public EduProgram GetById(string id, bool includeEducationalYear)
        {
            if (includeEducationalYear)
                return _context.EduPrograms.Include(x => x.DescriptionFile).Include(x => x.EducationalYear).FirstOrDefault(p => p.EduProgramId == id);
            else
                return this.GetById(id);
        }

        public IQueryable<EduProgram> GetForMonth(DateTime dateContainsRequiredMonth)
        {
            var year = _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            return _context.EduPrograms
                .Include(x => x.DescriptionFile)
                .Include(x => x.SchoolLevels)
                .Where(
                x => x.EducationalYearId == year.EducationalYearId &&
                DateTime.Compare(x.ReportDeadline, dateContainsRequiredMonth) > -1)
                .OrderBy(x => x.EndDate).AsNoTracking();
        }

        public bool IsDeadlinePassed(EduProgram program, DateTime currentDate)
        {
            return DateTime.Compare(program.ReportDeadline, currentDate) <= 0;
        }

        public EduProgram Remove(EduProgram entity)
        {
            _context.EduPrograms.Remove(entity);
            return entity;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
