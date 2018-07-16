
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
    public class AiSuggestionsRepository
    {
        string SUGGESTIONS_SEARCH = "SuggestionsSearch";
        MongoFactory _mongoFactory;
        AppSettings _appSettings;
        public AiSuggestionsRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _mongoFactory = new MongoFactory(appSettings);
        }

      

        public async Task<List<BsonDocument>> GetByBusiness(string business_id)
        {
            try
            {
                var options = new FindOptions<BsonDocument, BsonDocument>();
                options.Projection = "{'_id': 0}";
                options.Limit = 500;
                //options.Sort = Builders<BsonDocument>.Sort.Descending("status");
                var query = "{}";

                return await _mongoFactory.excuteMongoSelect(query, options,
                 MongoFactory._mongoClientAi, _appSettings.MongoDB.AiDb, SUGGESTIONS_SEARCH);
            }
            catch (Exception ex) { return null; }
        }

    }
}
