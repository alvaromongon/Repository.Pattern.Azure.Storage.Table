using Microsoft.Azure.Storage.Table.Repository.Tests.DomainModel;
using Microsoft.WindowsAzure.Storage.Table;
using Repository.Pattern.Abstractions;

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
            return $"{entity.AString}";
        }

        protected override string GetRowKey(DomainModelClass entity)
        {
            return $"{ entity.AnotherString}_{entity.AGuid.ToString()}";
        }
    }
}
