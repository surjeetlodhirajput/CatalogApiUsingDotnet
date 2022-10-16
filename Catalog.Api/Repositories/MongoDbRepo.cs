using System;
using System.Collections.Generic;
using  Catalog.Api.Entities;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
namespace Catalog.Api.Repositories{
    public class MongoDbRepo : IInMemItemRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        public MongoDbRepo(IMongoClient mongoClient)
        {   
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemCollection = database.GetCollection<Item>(collectionName);

        }
        public async Task CreateItemAsync(Item item)
        {
          await itemCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
         var filter = filterBuilder.Eq(it => it.Id,id);
         await itemCollection.DeleteOneAsync(filter);           
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await itemCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id,id);
          return await itemCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(it => it.Id,item.Id);
           await itemCollection.ReplaceOneAsync(filter,item);
        }
    }
}