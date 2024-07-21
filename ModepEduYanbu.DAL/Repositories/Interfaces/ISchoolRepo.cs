using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface ISchoolRepo:IRepository<School>
    {
        School GetByMinistryNo(string ministryNo, bool includeUsersForSchool = false);
        IQueryable<School> GetAllByAuthorizedPersonIdNo(string personIdNo, bool includeUsersForSchool = false);
        List<School> GetAllForUser(string username, bool includeUsersForSchool = false);
        School UpdatePrincipleIdNo(string schoolId, string newPrincipleId);
        School UpdatePrincipleIdNo(School school, string newPrincipleId);
        ApplicationUser GetPrinciple(string schoolId);
        ApplicationUser GetPrinciple(School school);
        bool IsSchoolMinistryNoValid(string ministryNo);
        bool SchoolIdMatchesMinistryNo(string schoolId, string schoolMinistryNo);
    }
}
