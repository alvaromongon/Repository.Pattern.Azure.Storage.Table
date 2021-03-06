﻿using Repository.Pattern.Azure.Storage.Table.IntegrationTests.DomainModel;
using Microsoft.WindowsAzure.Storage.Table;
using Repository.Pattern.Abstractions;

namespace Repository.Pattern.Azure.Storage.Table.IntegrationTests.Repository
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
