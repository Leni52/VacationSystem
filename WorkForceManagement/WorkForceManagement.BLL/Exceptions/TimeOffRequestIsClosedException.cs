using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class TimeOffRequestIsClosedException : Exception
    {
        public TimeOffRequestIsClosedException() : base() { }

        public TimeOffRequestIsClosedException(string message) : base(message) { }

        public TimeOffRequestIsClosedException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
