using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace MedPark.Common.Mongo
{
    public class MongoDbSeeder : IMongoDbSeeder
    {
        protected readonly IMongoDatabase Database;

        public MongoDbSeeder(IMongoDatabase database)
        {
            Database = database;
        }

        public async Task SeedAsync()
        {

            await CustomSeedAsync();
        }

        protected virtual async Task CustomSeedAsync()
        {
            var cursor = await Database.ListCollectionsAsync();

            var collections = await cursor.ToListAsync();

            if (collections.Any())
            {
                return;
            }

            await Task.CompletedTask;
        }
    }
}