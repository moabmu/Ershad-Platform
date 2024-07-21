using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity:class
    {
        IEnumerable<TEntity> GetAll();
        //IQueryable<TEntity> Query { get; }
        TEntity GetById(string id);
        TEntity Add(TEntity entity);
        TEntity Remove(TEntity entity);
        Task<bool> SaveChangesAsync();
    }
}
