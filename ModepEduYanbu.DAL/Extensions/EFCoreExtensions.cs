using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModepEduYanbu.DAL.Extensions
{
    public static class EFCoreExtensions
    {
        // get DbSet by its type.
        // https://stackoverflow.com/questions/47910340/how-to-get-dbset-from-entity-name-in-ef-core-net-core-2-0
        // https://stackoverflow.com/questions/4667981/c-sharp-use-system-type-as-generic-parameter
        // https://stackoverflow.com/questions/21533506/find-a-specified-generic-dbset-in-a-dbcontext-dynamically-when-i-have-an-entity/21533745
        /// <summary>
        /// E.g.: _context.GetDbSet(typeof(ReportKpiForMentor)).FirstOrDefault();
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        //public static IQueryable<object> GetDbSet(this DbContext _context, Type t)
        //{
        //    return (IQueryable<object>)_context.GetType().GetMethod("Set").MakeGenericMethod(t).Invoke(_context, null);
        //}
        public static IQueryable<TEntity> GetDbSet<TEntity>(this DbContext _context, Type t)
        {
            return (IQueryable<TEntity>)_context.GetType().GetMethod("Set").MakeGenericMethod(t).Invoke(_context, null);
        }

        /// <summary>
        /// Example:
        /// foreach (var client in clientList.OrderBy(c => c.Id).QueryInChunksOf(100))
        /// {
        ///     context.SaveChanges();
        /// }
        /// For more information: https://stackoverflow.com/questions/2113498/sqlexception-from-entity-framework-new-transaction-is-not-allowed-because-ther
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryInChunksOf<T>(this IQueryable<T> queryable, int chunkSize)
        {
            return queryable.QueryChunksOfSize(chunkSize).SelectMany(chunk => chunk);
        }

        public static IEnumerable<T[]> QueryChunksOfSize<T>(this IQueryable<T> queryable, int chunkSize)
        {
            int chunkNumber = 0;
            while (true)
            {
                var query = chunkNumber == 0
                    ? queryable
                    : queryable.Skip(chunkNumber * chunkSize);
                var chunk = query.Take(chunkSize).ToArray();
                if (chunk.Length == 0)
                    yield break;
                yield return chunk;
                chunkNumber++;
            }
        }


        //public static IQueryable Set(this DbContext context, Type T)
        //{

        //    // Get the generic type definition
        //    MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

        //    // Build a method with the specific type argument you're interested in
        //    method = method.MakeGenericMethod(T);

        //    return method.Invoke(context, null) as IQueryable;
        //}

        //public static IQueryable<T> GetDbSet<T>(this DbContext context)
        //{
        //    // Get the generic type definition 
        //    MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

        //    // Build a method with the specific type argument you're interested in 
        //    method = method.MakeGenericMethod(typeof(T));

        //    return method.Invoke(context, null) as IQueryable<T>;
        //}
    }
}
