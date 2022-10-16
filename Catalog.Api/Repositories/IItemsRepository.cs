using System.Collections.Generic;
using  Catalog.Api.Entities;
using System;
using System.Threading.Tasks;
namespace Catalog.Api.Repositories
{
    public interface IInMemItemRepository
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<Item> GetItemAsync(Guid id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }

}