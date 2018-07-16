using Ai.Data.Providers.Mongo;
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

namespace Ai.Service
{
    public class ProceduceAiService
    {

        AppSettings _appSettings;
        public ProceduceAiService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<BsonDocument> Proceduce(string para)
        {
            try
            {
                try
                {
                    para = para.Substring(0, 1) == "\"" ? para : ("\"" + para + "\"");
                    var db = MongoFactory._mongoClientAi.GetDatabase(_appSettings.MongoDB.AiDb);
                    var cmd = new JsonCommand<BsonDocument>("{ eval: " + para.Replace("\r\n","") + "}");
                    return await db.RunCommandAsync<BsonDocument>(cmd);
                    //var data = rs.ToList();
                    //if (data != null && data.Count > 1)
                    //{
                    //    return data[0].Value.ToString();
                    //}
                    //return JsonConvert.SerializeObject(data);
                }
                catch (Exception ex) { return null; }
            }
            catch (Exception ex) { return null; }
        }

    }
}
