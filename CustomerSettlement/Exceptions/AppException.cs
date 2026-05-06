using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }
    public class CloneException : AppException
    {
        public CloneException(string message) : base(message) { }
    }
    public class DeleteRestrictedException(string message) : AppException(message) {}
}
