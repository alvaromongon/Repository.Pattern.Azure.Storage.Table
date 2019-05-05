using Microsoft.Azure.Storage.Table.Repository.Tests.Configuration;
using Microsoft.Azure.Storage.Table.Repository.Tests.DomainModel;
using Microsoft.Azure.Storage.Table.Repository.Tests.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Repository.Pattern.Abstractions;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Table.Repository.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private const string settingsFile = "local.settings.json";

        private static IRepository<DomainModelClass> _sut;

        [ClassInitialize]
        public void ClassInitialize()
        {
            var configurationRoot = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile(settingsFile, optional: true, reloadOnChange: false)
                    .Build();

            var cloudStorageAccount = CloudStorageAccount.Parse(configurationRoot.GetValue<string>("AzureStorageConnectionString"));
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            IRepositoryConfiguration <DomainModelClass> configuration = new DomainModelClassRepositoryConfiguration(configurationRoot);
            _sut = new DomainModelClassRepository(cloudTableClient, configuration);
        }

        [TestMethod]
        public async Task When_Then()
        {
            // Arrange
            

            //Act
            

            // Assert
            
        }
    }
}
