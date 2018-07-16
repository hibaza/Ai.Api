
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Ai.Domain;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Ai.Data.Providers.Mongo;

namespace Ai.Data.Repositories.Mongo
{
    public class ProductsRepository
    {
        string PRODUCTS = "Products";
        MongoFactory _mongoFactory;
        AppSettings _appSettings;
        public ProductsRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _mongoFactory = new MongoFactory(appSettings);
        }


        public async Task<BsonDocument> GetById(string business_id, string id)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                //options.Limit = 100;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'" + business_id + "',id:'" + id + "'}";

                var list = await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);
                if (list != null && list.Count > 0)
                    return list[0];
                return null;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<BsonDocument>> GetParameterDynamic(string query)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                //options.Limit = 100;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
               
                return await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<BsonDocument>> GetFullTextSearch(string query,int next)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                options.Limit = 50;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");

                return await _mongoFactory.excuteMongoFullTextSearch(query, next,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<BsonDocument> SearchProduct(string business_id, string product)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'" + business_id + "','product.value':{ $regex: '"+product+"'}}";               
                var list = await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);
                if (list != null && list.Count > 0)
                    return list[0];
                return null;
            }
            catch (Exception ex) { return null; }
        }


        public async Task<List<BsonDocument>> SearchProducts(string business_id, string product)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1000;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'" + business_id + "','product.value':{ $regex: '" + product + "'}}";
                
                return await _mongoFactory.excuteMongoSelect(query, options,
                  MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);

            }
            catch (Exception ex) { return null; }
        }

        public async Task<ReplaceOneResult> upsert(BsonDocument data)
        {
            try
            {
                if (!data.Contains("business_id") || string.IsNullOrWhiteSpace((string)data["business_id"]))
                    return null;

                var option = new UpdateOptions { IsUpsert = true };
                var filter = Builders<BsonDocument>.Filter.Where(x => x["id"] == data["id"]&
                    x["business_id"] == data["business_id"]);

                return await _mongoFactory.excuteMongoUpdate(filter, data, option,
         MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTS);

            }
            catch (Exception ex) { return null; }
        }

    }
}
