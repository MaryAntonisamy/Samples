
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICosmosDbRepository<T> where T : class
{
    Task<T> GetByIdAsync(string id, string partitionKeyValue);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T item);
    Task UpdateAsync(string id, T item, string partitionKeyValue);
    Task DeleteAsync(string id, string partitionKeyValue);
}
    