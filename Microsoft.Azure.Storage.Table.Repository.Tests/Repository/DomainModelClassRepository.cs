using Microsoft.Azure.Storage.Table.Repository.Tests.DomainModel;
using Microsoft.WindowsAzure.Storage.Table;
using Repository.Pattern.Abstractions;
using System;

namespace Microsoft.Azure.Storage.Table.Repository.Tests.Repository
{
    public class DomainModelClassRepository : AzureTableStorageRepository<DomainModelClass>
    {
        public DomainModelClassRepository(CloudTableClient cloudTableClient, IRepositoryConfiguration<DomainModelClass> repositoryConfiguration) 
            : base(cloudTableClient, repositoryConfiguration)
        {
        }

        protected override string GetPartitionKey(DomainModelClass entity)
        {
            throw new NotImplementedException();
        }

        protected override string GetRowKey(DomainModelClass entity)
        {
            throw new NotImplementedException();
        }
    }
}
