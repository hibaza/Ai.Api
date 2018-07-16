
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
    public class TemplatesRepository
    {
        string TEMPLATES = "Templates";
        MongoFactory _mongoFactory;
        AppSettings _appSettings;
        public TemplatesRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _mongoFactory = new MongoFactory(appSettings);
        }

      
        public async Task<BsonDocument> GetById(string business_id,string id)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                //options.Limit = 100;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'"+ business_id + "',id:'"+id+"'}";

                var list = await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, TEMPLATES);
                if (list != null && list.Count > 0)
                    return list[0];
                return null;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<BsonDocument>> GetByBusiness(string business_id)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                //options.Limit = 100;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'" + business_id + "'}";

                return await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, TEMPLATES);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<BsonDocument>> GetList(string business_id)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0,id:1,desc:1}";
                options.Limit = 50;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{business_id:'" + business_id + "'}";

                return await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, TEMPLATES);
            }
            catch (Exception ex) { return null; }
        }
    }
}
