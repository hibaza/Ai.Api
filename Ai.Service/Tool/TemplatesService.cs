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
    public class TemplatesService
    {

        AppSettings _appSettings;
        TemplatesRepository _templatesRepository;

        public TemplatesService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _templatesRepository = new TemplatesRepository(appSettings);
        }
        
        public async Task<string> GetList(string business_id)
        {
            var rs = await _templatesRepository.GetList(business_id);
            return rs.ToJson();
        }
    }
}
