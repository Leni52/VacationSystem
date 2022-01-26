using System;

namespace WorkForceManagement.BLL.Exceptions
{
    public class CannotAddFileIfTORIsNotSickLeaveException : Exception
    {
        public CannotAddFileIfTORIsNotSickLeaveException()
        {
        }

        public CannotAddFileIfTORIsNotSickLeaveException(
            string message) : base(message)
        {
        }
    }
}
