
using Ai.Data.Repositories.Mongo;
using Ai.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai.Service
{
    public class WitTokenService
    {
        AppSettings _appSettings;
        WitTokenRepository _witTokenRepository;
        public WitTokenService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _witTokenRepository = new WitTokenRepository(appSettings);
        }

        public async Task<BsonDocument> GetById(string business_id)
        {
            return await _witTokenRepository.GetById(business_id);
        }
    }
}
