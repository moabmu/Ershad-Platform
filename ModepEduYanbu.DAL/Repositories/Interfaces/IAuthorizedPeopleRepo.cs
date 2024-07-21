using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IAuthorizedPeopleRepo:IRepository<AuthorizedPerson>
    {
        AuthorizedPerson GetByIdNo(string idNo);
        IQueryable<AuthorizedPerson> GetBySchoolMinistryNo(string schoolMinistryNo);
        bool IsIdNoValid(string idNo);
        bool PersonMatchesSchoolMinistryNo(string personIdNo, string schoolMinistryNo);
        bool IsEduDepEmployee(string idNo);
        IEnumerable<AuthorizedPerson> GetAllRecordsByIdNo(string idNo);
        void RemoveAllRecordsByIdNo(string idNo);
        Task<bool> CanAdd(AuthorizedPerson entity);
        Task<bool> CanAdd(string idNo);
    }
}
