using System;
using System.Runtime.Serialization;

namespace WorkForceManagement.BLL.Exceptions
{
    namespace WorkForceManagement.BLL
    {
        [Serializable]
       public class InvalidIdException : Exception
        {
            public InvalidIdException()
            {
            }

            public InvalidIdException(string message) : base(message)
            {
            }

            public InvalidIdException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected InvalidIdException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}