using System;

namespace Microsoft.Azure.Storage.Table.Repository.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
