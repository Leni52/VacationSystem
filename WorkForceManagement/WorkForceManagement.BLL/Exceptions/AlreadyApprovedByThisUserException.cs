using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL.Services
{
    [Serializable]
    internal class AlreadyApprovedByThisUserException : Exception
    {
        public AlreadyApprovedByThisUserException()
        {
        }

        public AlreadyApprovedByThisUserException(string message) : base(message)
        {
        }

        public AlreadyApprovedByThisUserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyApprovedByThisUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}