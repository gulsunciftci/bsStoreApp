using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryBase<T>
    {
        //Sorgulanabilir ifadeler
        //değişiklikleri izleyip izlememek için bunu bir parametreye bağlıyoruz trackChanges bunu ifade ediyor
        //CRUD
        IQueryable<T> FindAll(bool trackChanges);

        //T:Generic
        //Func:Delege
        IQueryable<T> FindByCondition(Expression<Func<T,bool>> expression,bool trackChanges);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    
    }
}
