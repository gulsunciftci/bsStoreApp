using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions.BadRequestException
{
    public abstract class BadRequestException : Exception //newlenemiyor
    {
        protected BadRequestException(string message) :
            base(message)
        {

        }
    }

}
