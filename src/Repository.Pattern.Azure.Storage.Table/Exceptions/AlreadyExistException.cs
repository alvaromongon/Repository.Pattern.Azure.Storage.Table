using System;

namespace Repository.Pattern.Azure.Storage.Table.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
