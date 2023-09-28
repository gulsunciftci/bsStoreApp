using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    //Generic class
    //abstract olması newlenememesi demek
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T: class //implemente ettik
    {
        //protected: sınıf ve alt sınıflar erişim sağlar
        protected readonly RepositoryContext _context;
        public RepositoryBase(RepositoryContext context)
        {
            _context = context;
        }

        public void Create(T entity) => _context.Set<T>().Add(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        //
        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ? _context.Set<T>().AsNoTracking() : //değişiklikleri izleme demek
            _context.Set<T>();


        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
               !trackChanges ?
               _context.Set<T>().Where(expression).AsNoTracking() : //değişiklikleri izleme demek
               _context.Set<T>().Where(expression); //değişiklikleri izle demek


        public void Update(T entity) => _context.Set<T>().Update(entity);

    }
}
