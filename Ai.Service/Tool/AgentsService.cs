using Ai.Data.Providers.Mongo;
using Ai.Data.Repositories.Mongo;
using Ai.Domain;
using Ai.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ai.Service.Tool
{
    public class AgentsService
    {

        AppSettings _appSettings;
        AgentsRepository _agentsRepository;

        public AgentsService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _agentsRepository = new AgentsRepository(appSettings);
        }
        
        public async Task<BsonDocument> GetById(string business_id,string id)
        {
            return await _agentsRepository.GetById(business_id,id);           
        }
        public async Task<List<BsonDocument>> GetByBusiness(string business_id)
        {
            return await _agentsRepository.GetByBusiness(business_id);
        }
        public async Task<BsonDocument> GetAgentByUserPass(string username, string password)
        {
            return await _agentsRepository.GetAgentByUserPass(username, password);
        }
    }
}
