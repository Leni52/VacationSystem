using System;
using System.Globalization;

namespace WorkForceManagement.BLL.Exceptions
{
    public class UsernameTakenException : Exception
    {
        public UsernameTakenException() : base() { }

        public UsernameTakenException(string message) : base(message) { }

        public UsernameTakenException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
