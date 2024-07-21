using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IEducationalYearsRepo : IRepository<EducationalYear>
    {
        EducationalYear GetCurrentYearOrCreateIfNotExist(bool includeEduPrograms = false);
    }
}
