using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class UserIsntApproverException : Exception
    {
        public UserIsntApproverException() : base() { }

        public UserIsntApproverException(string message) : base(message) { }

        public UserIsntApproverException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
