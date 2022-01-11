using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class TeamWithSameNameExistsException : Exception
    {
        public TeamWithSameNameExistsException() : base() { }

        public TeamWithSameNameExistsException(string message) : base(message) { }

        public TeamWithSameNameExistsException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

}
