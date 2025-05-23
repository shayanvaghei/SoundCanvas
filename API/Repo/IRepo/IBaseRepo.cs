using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repo.IRepo
{
    public interface IBaseRepo<T> where T : class
    {
        void Add(T entity);
        void Update(T source, T destination);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> criteria, string includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null, string includeProperties = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderedBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>> criteria = null);
    }
}
