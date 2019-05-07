using Repository.Pattern.Abstractions.Batches;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Pattern.Abstractions
{
    public interface IRepository<TDomainModel> where TDomainModel : class, new()
    {
        Task<IEnumerable<TDomainModel>> GetAllAsync();

        Task<IEnumerable<TDomainModel>> GetAllAsync(string partitionKey);

        Task<TDomainModel> GetAsync(string partitionKey, string rowKey);

        Task<TDomainModel> AddAsync(TDomainModel domainModel);

        Task AddBatchAsync(IEnumerable<TDomainModel> domainModelEnumerable, BatchOperationOptions options = null);

        Task<TDomainModel> AddOrUpdateAsync(TDomainModel domainModel);

        Task<TDomainModel> UpdateAsync(TDomainModel domainModel);        

        Task DeleteAllAsync(string partitionKey);

        Task<TDomainModel> DeleteAsync(TDomainModel domainModel);

        Task<TDomainModel> DeleteAsync(string partitionKey, string rowKey);
    }        
}
