using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class InvalidDatesException : Exception
    {
        public InvalidDatesException() : base() { }

        public InvalidDatesException(string message) : base(message) { }

        public InvalidDatesException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
