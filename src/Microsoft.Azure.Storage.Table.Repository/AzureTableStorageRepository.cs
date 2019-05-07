using Microsoft.Azure.Storage.Table.Repository.Exceptions;
using Microsoft.Azure.Storage.Table.Repository.TableEntities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Repository.Pattern.Abstractions;
using Repository.Pattern.Abstractions.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Table.Repository
{
    /// <summary>
    /// Using last implementation of Azure table storage in WindowsAzure.Storage 9.3.3
    /// 
    /// Based on https://github.com/Crokus/azure-table-storage-repo
    /// 
    /// Leveraging https://github.com/mgravell/fast-member
    /// </summary>
    /// <typeparam name="T">A Mode object not related with Table storage at all</typeparam>
    public abstract class AzureTableStorageRepository<TDomainModel> : IRepository<TDomainModel> where TDomainModel : class, new()
    {
        private const string PartitionKey = "PartitionKey";

        protected CloudTable Table => _tableLazy.Value;
        private readonly Lazy<CloudTable> _tableLazy;

        private readonly CloudTableClient _client;
        private readonly IRepositoryConfiguration<TDomainModel> _repositoryConfiguration;
        private readonly TableEntityAutoMapper<TDomainModel> _tableEntityAutoMapper;
        private readonly EntityResolver<TDomainModel> _entityResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorage" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AzureTableStorageRepository(CloudTableClient cloudTableClient, IRepositoryConfiguration<TDomainModel> repositoryConfiguration)
        {
            _client = cloudTableClient;
            _repositoryConfiguration = repositoryConfiguration;
            _tableEntityAutoMapper = new TableEntityAutoMapper<TDomainModel>();
            _entityResolver = _tableEntityAutoMapper.ToDomainModel;

            _tableLazy = new Lazy<CloudTable>(CreateCloudTable);
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync()
        {
            TableContinuationToken token = null;
            var entities = new List<TDomainModel>();
            do
            {                              
                var queryResult = await Table.ExecuteQuerySegmentedAsync<TDomainModel>(new TableQuery(), _entityResolver, token)
                    .ConfigureAwait(false);

                entities.AddRange(queryResult.Results);

                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync(string partitionKey)
        {
            var tableQuery = new TableQuery()
                .Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            TableContinuationToken token = null;
            var entities = new List<TDomainModel>();
            do
            {
                var queryResult = await Table.ExecuteQuerySegmentedAsync<TDomainModel>(tableQuery, _entityResolver, token)
                    .ConfigureAwait(false);

                entities.AddRange(queryResult.Results);

                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public async Task<TDomainModel> GetAsync(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TDomainModel>(partitionKey, rowKey, _entityResolver);

                TableResult result = await Table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

                return result.Result as TDomainModel;
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Retrieve);
            }            
        }

        public async Task<TDomainModel> AddAsync(TDomainModel domainModel)
        {
            GuardDomainModel(domainModel);

            try
            {                
                var tableEntity = _tableEntityAutoMapper.ToTableEntity(domainModel, GetPartitionKey(domainModel), GetRowKey(domainModel));

                TableOperation insertOperation = TableOperation.Insert(tableEntity);

                TableResult result = await Table.ExecuteAsync(insertOperation).ConfigureAwait(false);

                return await GetAsync(tableEntity.PartitionKey, tableEntity.RowKey).ConfigureAwait(false);
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Insert);
            }
        }

        public async Task AddBatchAsync(IEnumerable<TDomainModel> domainModelEnumerable, BatchOperationOptions options = null)
        {
            GuardDomainModels(domainModelEnumerable);

            try
            {                
                options = options ?? new BatchOperationOptions();

                var tasks = new List<Task<IList<TableResult>>>();

                const int addBatchOperationLimit = 100;
                var entitiesOffset = 0;
                while (entitiesOffset < domainModelEnumerable?.Count())
                {
                    var entitiesToAdd = domainModelEnumerable.Skip(entitiesOffset).Take(addBatchOperationLimit).ToList();
                    entitiesOffset += entitiesToAdd.Count;

                    Action<TableBatchOperation, ITableEntity> batchInsertOperation = null;
                    switch (options.BatchInsertMethod)
                    {
                        case BatchInsertMethod.Insert:
                            batchInsertOperation = (bo, tableEntity) => bo.Insert(tableEntity);
                            break;
                        case BatchInsertMethod.InsertOrReplace:
                            batchInsertOperation = (bo, tableEntity) => bo.InsertOrReplace(tableEntity);
                            break;
                        case BatchInsertMethod.InsertOrMerge:
                            batchInsertOperation = (bo, tableEntity) => bo.InsertOrMerge(tableEntity);
                            break;
                    }
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    foreach (var domainModel in entitiesToAdd)
                    {
                        var tableEntity = _tableEntityAutoMapper.ToTableEntity(domainModel, GetPartitionKey(domainModel), GetRowKey(domainModel));

                        batchInsertOperation(batchOperation, tableEntity);
                    }
                    tasks.Add(Table.ExecuteBatchAsync(batchOperation));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Insert);
            }
        }

        public async Task<TDomainModel> AddOrUpdateAsync(TDomainModel domainModel)
        {
            GuardDomainModel(domainModel);

            try
            {
                var tableEntity = _tableEntityAutoMapper.ToTableEntity(domainModel, GetPartitionKey(domainModel), GetRowKey(domainModel));

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(tableEntity);

                TableResult result = await Table.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);

                return await GetAsync(tableEntity.PartitionKey, tableEntity.RowKey).ConfigureAwait(false);
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.InsertOrReplace);
            }            
        }              

        public async Task<TDomainModel> UpdateAsync(TDomainModel domainModel)
        {
            GuardDomainModel(domainModel);

            try
            {
                var tableEntity = _tableEntityAutoMapper.ToTableEntity(domainModel, GetPartitionKey(domainModel), GetRowKey(domainModel));

                TableOperation replaceOperation = TableOperation.Replace(tableEntity);

                TableResult result = await Table.ExecuteAsync(replaceOperation).ConfigureAwait(false);

                return await GetAsync(tableEntity.PartitionKey, tableEntity.RowKey).ConfigureAwait(false);
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Replace);
            }
        }

        public async Task DeleteAllAsync(string partitionKey)
        {
            var tableBatchOperation = new TableBatchOperation();

            var tableQuery = new TableQuery()
                .Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            TableContinuationToken token = null;
            var entities = new List<TDomainModel>();
            do
            {
                var queryResult = await Table.ExecuteQuerySegmentedAsync(tableQuery, token)
                    .ConfigureAwait(false);

                foreach(var result in queryResult.Results)
                {
                    tableBatchOperation.Add(TableOperation.Delete(result));
                }

                token = queryResult.ContinuationToken;
            } while (token != null);
        }

        public async Task<TDomainModel> DeleteAsync(TDomainModel domainModel)
        {
            GuardDomainModel(domainModel);            

            try
            {
                var tableEntity = _tableEntityAutoMapper.ToTableEntity(domainModel, GetPartitionKey(domainModel), GetRowKey(domainModel));

                TableOperation deleteOperation = TableOperation.Delete(tableEntity);

                TableResult result = await Table.ExecuteAsync(deleteOperation).ConfigureAwait(false);

                return domainModel;
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Delete);
            }
        }        

        public async Task<TDomainModel> DeleteAsync(string partitionKey, string rowKey)
        {            
            try
            {
                var table = _client.GetTableReference(_repositoryConfiguration.TableName);

                TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
                TableResult retrieveOperationResult = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

                TableOperation deleteOperation = TableOperation.Delete(retrieveOperationResult.Result as ITableEntity);
                TableResult deleteOperationResult = await table.ExecuteAsync(deleteOperation).ConfigureAwait(false);

                return retrieveOperationResult.Result as TDomainModel;
            }
            catch (StorageException exception)
            {
                throw HandleException(exception, TableOperationType.Delete);
            }
        }

        private CloudTable CreateCloudTable()
        {
            var result = _client.GetTableReference(_repositoryConfiguration.TableName);            
            result.CreateIfNotExistsAsync().Wait();

            return result;
        }
        private static void GuardDomainModel(TDomainModel domainModel)
        {
            if (domainModel == null)
            {
                throw new ArgumentNullException(nameof(domainModel));
            }
        }
        private static void GuardDomainModels(IEnumerable<TDomainModel> domainModelEnumerable)
        {
            if (domainModelEnumerable == null)
            {
                throw new ArgumentNullException(nameof(domainModelEnumerable));
            }
        }
        private static Exception HandleException(StorageException exception, TableOperationType type)
        {
            var statusCode = (HttpStatusCode)exception.RequestInformation.HttpStatusCode;
            var errorCode = exception.RequestInformation.ExtendedErrorInformation.ErrorCode;

            if (type == TableOperationType.Insert || type == TableOperationType.InsertOrMerge || type == TableOperationType.InsertOrReplace)
            {
                if (statusCode == HttpStatusCode.Conflict && errorCode == "EntityAlreadyExists")
                {
                    return new AlreadyExistException("The given entity already exists in the table", exception);
                }
            }
            else if (type == TableOperationType.Merge || type == TableOperationType.Replace || type == TableOperationType.Delete || type == TableOperationType.Retrieve)
            {
                if (statusCode == HttpStatusCode.Conflict && errorCode == StorageErrorCodeStrings.ResourceNotFound)
                {
                    return new DoesNotExistException("The given entity does not exists in the table", exception);
                }
            }

            return new Exception(exception.Message, exception);
        }

        protected abstract string GetPartitionKey(TDomainModel entity);
        protected abstract string GetRowKey(TDomainModel entity);
    }
}
