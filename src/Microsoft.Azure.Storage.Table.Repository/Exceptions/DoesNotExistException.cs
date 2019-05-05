using System;

namespace Microsoft.Azure.Storage.Table.Repository.Exceptions
{
    public class DoesNotExistException : Exception
    {
        public DoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
