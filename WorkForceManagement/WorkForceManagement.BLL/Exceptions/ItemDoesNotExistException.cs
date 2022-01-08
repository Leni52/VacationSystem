using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL
{
    [Serializable]
    internal class ItemDoesNotExistException : Exception
    {
        public ItemDoesNotExistException()
        {
        }

        public ItemDoesNotExistException(string message) : base(message)
        {
        }

        public ItemDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ItemDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}