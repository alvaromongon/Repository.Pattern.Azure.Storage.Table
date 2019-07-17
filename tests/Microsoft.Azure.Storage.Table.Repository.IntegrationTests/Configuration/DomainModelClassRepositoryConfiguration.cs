using Microsoft.Azure.Storage.Table.Repository.IntegrationTests.DomainModel;
using Microsoft.Extensions.Configuration;
using Repository.Pattern.Abstractions;

namespace Microsoft.Azure.Storage.Table.Repository.IntegrationTests.Configuration
{
    public class DomainModelClassRepositoryConfiguration : IRepositoryConfiguration<DomainModelClass>
    {
        private readonly IConfigurationRoot _configurationRoot;

        public DomainModelClassRepositoryConfiguration(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public string TableName => _configurationRoot.GetValue<string>("TableTestName");
    }
}
