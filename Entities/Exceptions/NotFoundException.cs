using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions //istisnaları yönetmek için
{
  
    public abstract class NotFoundException : Exception
    {
        //sealed: kalıtılması mümkün değil demek
        public NotFoundException(string message) : base(message)
        {

        }
    }
}

