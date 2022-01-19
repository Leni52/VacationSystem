using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class OverlappingTimeOffRequestsException : Exception
    {
        public OverlappingTimeOffRequestsException() : base() { }

        public OverlappingTimeOffRequestsException(string message) : base(message) { }

        public OverlappingTimeOffRequestsException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
