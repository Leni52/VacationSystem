﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.BLL.Exceptions
{
    public class UserEmailNotConfirmedException : Exception
    {
        public UserEmailNotConfirmedException() : base() { }

        public UserEmailNotConfirmedException(string message) : base(message) { }

        public UserEmailNotConfirmedException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
