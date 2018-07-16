
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
    public class ProductGeneralRepository
    {
        string PRODUCTGENERAL = "ProductGeneral";
        MongoFactory _mongoFactory;
        AppSettings _appSettings;
        public ProductGeneralRepository(AppSettings appSettings)
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
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTGENERAL);
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
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, PRODUCTGENERAL);
            }
            catch (Exception ex) { return null; }
        }
    }
}
