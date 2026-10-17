using System.Linq.Expressions;

namespace Football.Repositry.IRepositry
{
    public interface IRepository <T> where T : class
    {
        T FindOne(int  id);
        T FindById(int id,params string[] agers);
       void AddOne(T item);
       void UpdateOne(T item);
       void RemoveOne(T item);
       void AddList(IEnumerable<T> item);
       void UpdateList(IEnumerable<T> item);
       void RemoveList(IEnumerable<T> item);

        T SelectOne(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params string[] agers);
    }
}
