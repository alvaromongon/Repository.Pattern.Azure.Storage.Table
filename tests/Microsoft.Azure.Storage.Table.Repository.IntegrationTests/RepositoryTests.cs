using FluentAssertions;
using Microsoft.Azure.Storage.Table.Repository.Exceptions;
using Microsoft.Azure.Storage.Table.Repository.IntegrationTests.Configuration;
using Microsoft.Azure.Storage.Table.Repository.IntegrationTests.DomainModel;
using Microsoft.Azure.Storage.Table.Repository.IntegrationTests.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Repository.Pattern.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Table.Repository.IntegrationTests
{
    [TestClass]
    public class RepositoryTests
    {
        private const string _settingsFile = "local.settings.json";
        private static string _partitionKeyString;

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

            _partitionKeyString = Guid.NewGuid().ToString();
        }

        [ClassCleanup]
        public static async Task ClassCleanUp()
        {
            await _sut.DeleteAllAsync(_partitionKeyString);
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
        public async Task WhenAdd_AndGetWithPartitionAndRowKey_ThenIsReturned()
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
        public async Task WhenAddBatch_AndGetAll_ThenAreReturned()
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
        public async Task WhenAddBatch_AndGetAllWithPartitionKey_ThenAreReturned()
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

        [TestMethod]
        public async Task WhenAddOrUpdate_AndNotExist_AndGet_ThenIsReturned()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            var addResult = await _sut.AddOrUpdateAsync(domainModel1);
            var result = await _sut.GetAsync(partitionKey, rowKey);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        public async Task WhenAddOrUpdate_AndExist_AndGet_ThenIsReturnedWithTheUpdate()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            var addResult1 = await _sut.AddOrUpdateAsync(domainModel1);
            domainModel1.ALong = 45234563456534;
            domainModel1.ADateTimeOffset = DateTimeOffset.UtcNow;
            var addResult2 = await _sut.AddOrUpdateAsync(domainModel1);

            var result = await _sut.GetAsync(partitionKey, rowKey);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        [ExpectedException(typeof(DoesNotExistException))]
        public async Task WhenUpdate_AndDoesNotExist_ThenDoesNotExistException()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            await _sut.UpdateAsync(domainModel1);

            // Assert
        }

        [TestMethod]
        public async Task WhenUpdate_AndGet_ThenIsReturnedWithTheUpdate()
        {
            // Arrange
            var domainModel1 = new DomainModelClass(_partitionKeyString);
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            var addResult1 = await _sut.AddAsync(domainModel1);
            domainModel1.ALong = 34523452;
            domainModel1.ADateTimeOffset = DateTimeOffset.UtcNow;
            var addResult2 = await _sut.UpdateAsync(domainModel1);

            var result = await _sut.GetAsync(partitionKey, rowKey);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(domainModel1);
        }

        [TestMethod]
        public async Task WhenAddBatch_AndDeleteAll_ThenAreDeleted()
        {
            // Arrange
            var suffix = "_TODELETEALL";
            var domainModel1 = new DomainModelClass($"{_partitionKeyString}{suffix}");
            var domainModel2 = new DomainModelClass($"{_partitionKeyString}{suffix}");
            var domainModel3 = new DomainModelClass($"{_partitionKeyString}{suffix}");

            var list = new List<DomainModelClass>()
            {
                domainModel1,
                domainModel2,
                domainModel3
            };
            var partitionKey = $"{domainModel1.AString}";

            //Act
            await _sut.AddBatchAsync(list);
            var getAllResult = await _sut.GetAllAsync(partitionKey);
            await _sut.DeleteAllAsync(partitionKey);
            var getAllResultAfterDelete = await _sut.GetAllAsync(partitionKey);

            // Assert
            getAllResult.Should().HaveCountGreaterOrEqualTo(3);
            getAllResultAfterDelete.Should().BeEmpty();
        }

        [TestMethod]
        public async Task WhenAddBatch_AndDeleteByEntity_ThenIsDeleted()
        {
            // Arrange
            var suffix = "_TODELETEBYENTITY";
            var domainModel1 = new DomainModelClass($"{_partitionKeyString}{suffix}");
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            await _sut.AddAsync(domainModel1);
            var getResult = await _sut.GetAsync(partitionKey, rowKey);
            await _sut.DeleteAsync(domainModel1);
            var getAfterDelete = await _sut.GetAllAsync(partitionKey);

            // Assert
            getResult.Should().NotBeNull();
            getAfterDelete.Should().BeEmpty();
        }

        [TestMethod]
        public async Task WhenAddBatch_AndDeleteByPartitionAndRowKey_ThenIsDeleted()
        {
            // Arrange
            var suffix = "_TODELETEBYPARTITION&ROWKEY";
            var domainModel1 = new DomainModelClass($"{_partitionKeyString}{suffix}");
            var partitionKey = $"{domainModel1.AString}";
            var rowKey = $"{ domainModel1.AnotherString}_{domainModel1.AGuid.ToString()}";

            //Act
            await _sut.AddAsync(domainModel1);
            var getResult = await _sut.GetAsync(partitionKey, rowKey);
            await _sut.DeleteAsync(partitionKey, rowKey);
            var getAfterDelete = await _sut.GetAllAsync(partitionKey);

            // Assert
            getResult.Should().NotBeNull();
            getAfterDelete.Should().BeEmpty();
        }
    }
}
