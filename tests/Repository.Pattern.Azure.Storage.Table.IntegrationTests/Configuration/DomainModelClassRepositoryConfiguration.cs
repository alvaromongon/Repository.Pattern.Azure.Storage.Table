using Repository.Pattern.Azure.Storage.Table.IntegrationTests.DomainModel;
using Microsoft.Extensions.Configuration;
using Repository.Pattern.Abstractions;

namespace Repository.Pattern.Azure.Storage.Table.IntegrationTests.Configuration
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
