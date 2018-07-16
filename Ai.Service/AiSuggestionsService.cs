
using Ai.Data.Repositories.Mongo;
using Ai.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai.Service
{
    public class AiSuggestionsService
    {
        AppSettings _appSettings;
        AiSuggestionsRepository _suggestionsRepository;
        public AiSuggestionsService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _suggestionsRepository = new AiSuggestionsRepository(appSettings);
        }

        public async Task<string> GetByBusiness(string business_id)
        {
            var list= await _suggestionsRepository.GetByBusiness(business_id);
            return list.ToJson();
        }
    }
}
