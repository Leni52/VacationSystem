using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL.Exceptions
{
    [Serializable]
    public class CannotCancelTimeOffRequestException : Exception
    {
        public CannotCancelTimeOffRequestException()
        {
        }

        public CannotCancelTimeOffRequestException(string message) : base(message)
        {
        }

        public CannotCancelTimeOffRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotCancelTimeOffRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
