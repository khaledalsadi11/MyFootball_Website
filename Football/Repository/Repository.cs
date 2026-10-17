using Football.Data;
using Football.Repositry.IRepositry;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Football.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context;
        public Repository(AppDbContext context) 
        {
            _context = context;
        }

        public void AddOne(T item)
        {
             _context.Set<T>().Add(item);
        }

        public void AddList(T item)
        {
            _context.Set<T>().AddRange(item);
        }

        public void RemoveOne(T item)
        {
            _context.Set<T>().Remove(item);
        }

        public void AddList(IEnumerable<T> items)
        {
            _context.Set<T>().AddRange(items);
        }
        public void UpdateOne(T item)
        {
            _context.Set<T>().Update(item);
        }
        public void UpdateList(IEnumerable<T> items)
        {
            _context.Set<T>().UpdateRange(items);
        }

        public void RemoveList(IEnumerable<T> items)
        {
            _context.Set<T>().RemoveRange(items);
        }


        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public IEnumerable<T> GetAll(params string[] agers)
        {
            IQueryable<T> query = _context.Set<T>();
            if(agers.Length > 0 ) 
            {
                foreach (var ager in agers)
                {
                    query = query.Include(ager);
                }
            }
            return query.ToList();
        }

        public T FindOne(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public T FindById(int id, params string[] agers)
        {
            IQueryable<T> query = _context.Set<T>();

            if (agers.Length > 0)
            {
                foreach (var ager in agers)
                {
                    query = query.Include(ager);
                }
            }

            return query.FirstOrDefault(x => EF.Property<int>(x, "Id") == id);
        }

        public T SelectOne(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);

        }
    }
}
