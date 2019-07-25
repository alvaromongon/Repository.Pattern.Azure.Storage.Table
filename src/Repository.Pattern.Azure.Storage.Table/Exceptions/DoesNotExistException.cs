using System;

namespace Repository.Pattern.Azure.Storage.Table.Exceptions
{
    public class DoesNotExistException : Exception
    {
        public DoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
