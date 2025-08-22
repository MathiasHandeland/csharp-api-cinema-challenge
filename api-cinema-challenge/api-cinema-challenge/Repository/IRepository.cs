using System.Linq.Expressions;

namespace api_cinema_challenge.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Delete(object id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);

        Task<IEnumerable<T>> GetWithIncludes(params Expression<Func<T, object>>[] includes);

    }
}
