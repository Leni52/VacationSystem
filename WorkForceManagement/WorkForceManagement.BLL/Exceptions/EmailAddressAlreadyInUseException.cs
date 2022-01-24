using System;
using System.Globalization;

namespace WorkForceManagement.BLL.Exceptions
{
    public class EmailAddressAlreadyInUseException : Exception
    {
        public EmailAddressAlreadyInUseException() : base() { }

        public EmailAddressAlreadyInUseException(string message) : base(message) { }

        public EmailAddressAlreadyInUseException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
