using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
       
        /// <summary>
        /// UnitofWork Design Pattern Nedir?
        /// Bu pattern, iş katmanında yapılan her değişikliğin anlık olarak database e yansıması yerine, 
        /// işlemlerin toplu halde tek bir kanaldan gerçekleşmesini sağlar.
        /// </summary>
        IBookRepository Book { get; }
        Task SaveAsync(); //tipi void ise task yalnız yazılabilir

    }
}
