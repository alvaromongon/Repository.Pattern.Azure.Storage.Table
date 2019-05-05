using Microsoft.Azure.Storage.Table.Repository.Tests.DomainModel;
using Microsoft.Extensions.Configuration;
using Repository.Pattern.Abstractions;
using System;

namespace Microsoft.Azure.Storage.Table.Repository.Tests.Configuration
{
    public class DomainModelClassRepositoryConfiguration : IRepositoryConfiguration<DomainModelClass>
    {
        private readonly IConfigurationRoot _configurationRoot;

        public DomainModelClassRepositoryConfiguration(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public string TableName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
