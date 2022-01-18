using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL.Services
{
    [Serializable]
    internal class InvalidRequestStatusException : Exception
    {
        public InvalidRequestStatusException()
        {
        }

        public InvalidRequestStatusException(string message) : base(message)
        {
        }

        public InvalidRequestStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidRequestStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}