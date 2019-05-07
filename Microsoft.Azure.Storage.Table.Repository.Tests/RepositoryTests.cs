using FluentAssertions;
using Microsoft.Azure.Storage.Table.Repository.Tests.Configuration;
using Microsoft.Azure.Storage.Table.Repository.Tests.DomainModel;
using Microsoft.Azure.Storage.Table.Repository.Tests.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Repository.Pattern.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Table.Repository.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private const string _settingsFile = "local.settings.json";
        private static readonly string _partitionKeyString = Guid.NewGuid().ToString();

        private static IRepository<DomainModelClass> _sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var configurationRoot = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile(_settingsFile, optional: false, reloadOnChange: false)
                    .Build();

            var cloudStorageAccount = CloudStorageAccount.Parse(configurationRoot.GetValue<string>("AzureStorageConnectionString"));
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            IRepositoryConfiguration <DomainModelClass> configuration = new DomainModelClassRepositoryConfiguration(configurationRoot);
            _sut = new DomainModelClassRepository(cloudTableClient, configuration);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sut.DeleteAllAsync(_partitionKeyString);
        }

        [TestMethod]
        public async Task WhenAdd_AndGetAll_ThenIsReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);

            //Act
            var addResult = await _sut.AddAsync(domainModel1);
            var getAllResult = await _sut.GetAllAsync();

            // Assert
            getAllResult.Should().NotBeNull();
            getAllResult.Should().HaveCountGreaterOrEqualTo(1);
            getAllResult.First(i => i.AnotherString == domainModel1.AnotherString).Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        public async Task WhenAdd_AndGetAllWithPartitionKey_ThenIsReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";

            //Act
            var addResult = await _sut.AddAsync(domainModel1);
            var getAllResult = await _sut.GetAllAsync(partitionKey);

            // Assert
            getAllResult.Should().NotBeNull();
            getAllResult.Should().HaveCountGreaterOrEqualTo(1);
            getAllResult.First(i => i.AnotherString == domainModel1.AnotherString).Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        public async Task WhenAdd_AndGet_ThenIsReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            var addResult = await _sut.AddAsync(domainModel1);
            var result = await _sut.GetAsync(partitionKey, rowKey);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task WhenAddBatch_AndDifferentPartition_ThenArgumentException()
        {
            // Arrange
            var domainModel1 = new DomainModelClass();
            var domainModel2 = new DomainModelClass();
            var list = new List<DomainModelClass>()
            {
                domainModel1,
                domainModel2
            };

            //Act
            await _sut.AddBatchAsync(list);
            
            // Assert
        }

        [TestMethod]
        public async Task WhenAdd_AndBatchGetAll_ThenAreReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var domainModel2 = new DomainModelClass(_partitionKeyString);

            var list = new List<DomainModelClass>()
            {
                domainModel1,
                domainModel2
            };
            var partitionKey = $"{domainModel1.AString}";

            //Act
            await _sut.AddBatchAsync(list);
            var getAllResult = await _sut.GetAllAsync();

            // Assert
            getAllResult.Should().NotBeNull();
            getAllResult.Should().HaveCountGreaterOrEqualTo(2);
            getAllResult.First(i => i.AnotherString == domainModel1.AnotherString).Should().BeEquivalentTo(domainModel1);
            getAllResult.First(i => i.AnotherString == domainModel2.AnotherString).Should().BeEquivalentTo(domainModel2);
        }

        [TestMethod]
        public async Task WhenAdd_AndBatchGetAllWithPartitionKey_ThenAreReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var domainModel2 = new DomainModelClass(_partitionKeyString);

            var list = new List<DomainModelClass>()
            {
                domainModel1,
                domainModel2
            };
            var partitionKey = $"{domainModel1.AString}";

            //Act
            await _sut.AddBatchAsync(list);
            var getAllResult = await _sut.GetAllAsync(partitionKey);

            // Assert
            getAllResult.Should().NotBeNull();
            getAllResult.Should().HaveCountGreaterOrEqualTo(2);
            getAllResult.First(i => i.AnotherString == domainModel1.AnotherString).Should().BeEquivalentTo(domainModel1);
            getAllResult.First(i => i.AnotherString == domainModel2.AnotherString).Should().BeEquivalentTo(domainModel2);
        }        
    }
}
