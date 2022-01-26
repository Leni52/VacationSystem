using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL.Exceptions
{
    [Serializable]
    public class InvalidTimeOffRequestTypeException : Exception
    {
        public InvalidTimeOffRequestTypeException()
        {
        }

        public InvalidTimeOffRequestTypeException(string message) : base(message)
        {
        }

        public InvalidTimeOffRequestTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTimeOffRequestTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}