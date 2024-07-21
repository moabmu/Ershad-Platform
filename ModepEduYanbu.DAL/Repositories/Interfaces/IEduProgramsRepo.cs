using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IEduProgramsRepo : IRepository<EduProgram>
    {
        EduProgram GetById(string id, bool includeEducationalYear);
        IQueryable<EduProgram> GetForMonth(DateTime dateContainsRequiredMonth);
        bool IsDeadlinePassed(EduProgram program, DateTime currentDate);
    }
}
