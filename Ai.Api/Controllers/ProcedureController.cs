using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ai.Core.TokenProviders;
using Ai.Domain;
using Ai.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Ai.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/procedure")]
    public class ProcedureController : Controller
    {

        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        
        public ProcedureController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        //[EnableCors("SiteCorsPolicy")]
        [HttpPost("execute")]
        [AllowAnonymous]
        public async Task<string> execute([FromBody] mongodb value)
        {
            try
            {
                var configDic = Core.Helpers.CommonHelper.JsonToBsonDocument(value.config);

                MongoClient client=null;
                IMongoDatabase db = null;
                if (((string)configDic["mongoconnect"]).ToLower() == "connai")
                {
                    client = new MongoClient(_appSettings.Value.MongoDB.ConnAi);
                    db = client.GetDatabase(_appSettings.Value.MongoDB.AiDb);
                }
                if (((string)configDic["mongoconnect"]).ToLower() == "connchat")
                {
                    client = new MongoClient(_appSettings.Value.MongoDB.ConnChat);
                    db = client.GetDatabase(_appSettings.Value.MongoDB.ChatDb);
                }                
                IMongoCollection<BsonDocument> collection = null;
                if (configDic.Contains("collectionname") && configDic["collectionname"] != "")
                    collection = db.GetCollection<BsonDocument>((string)configDic["collectionname"]);

                var type = ((string)configDic["type"]).ToLower();
                if (type == "insert")
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject(value.para).ToString().Replace("\"", "'");
                        var document = BsonSerializer.Deserialize<BsonDocument>(json);
                        collection.InsertOneAsync(document).Wait();
                        return "[]";
                    }
                    catch (Exception ex) { return null; }
                }
                if (type == "searchlike")
                {
                    try
                    {
                        string project = "{}";
                        if (value.projection != null)
                            project = value.projection;

                        var options = new FindOptions<BsonDocument>();
                        options.Projection = project;
                        options.Limit = value.limit == 0 ? 10 : value.limit;

                        var json = JsonConvert.DeserializeObject(value.para).ToString().Replace("\"", "'");
                        var document = BsonSerializer.Deserialize<BsonDocument>(json);
                        var data = await collection.FindAsync<BsonDocument>(document, options);
                        return JsonConvert.SerializeObject(data.ToList());
                    }
                    catch (Exception ex) { return null; }
                }
                if (type == "search")
                {
                    try
                    {
                        var options = new FindOptions<BsonDocument>();
                        if (value.projection != null)
                            options.Projection = value.projection;
                        options.Limit = value.limit == 0 ? 10 : value.limit;
                        if (value.order != null)
                        {
                            var js = JsonConvert.DeserializeObject(value.order).ToString().Replace("\"", "'");
                            var doc = BsonSerializer.Deserialize<BsonDocument>(js);
                            if (doc.Contains("asc"))
                                options.Sort = Builders<BsonDocument>.Sort.Ascending(doc["asc"].ToString());
                            else
                                options.Sort = Builders<BsonDocument>.Sort.Descending(doc["desc"].ToString());
                        }
                        var json = JsonConvert.DeserializeObject(value.para).ToString().Replace("\"", "'");
                        var document = BsonSerializer.Deserialize<BsonDocument>(json);
                        var data = await collection.FindAsync<BsonDocument>(document, options);
                        var rs = data.ToList();

                        string tt = rs.ToJson();
                        return tt;
                    }
                    catch (Exception ex) { return null; }
                }

                if (type == "procedure")
                {
                    try
                    {
                        var cmd = new JsonCommand<BsonDocument>("{ eval: " + value.para + "}");
                        var rs = await db.RunCommandAsync<BsonDocument>(cmd);
                        var data = rs.ToList();
                        if (data != null && data.Count > 1)
                        {
                            return data[0].Value.ToString();
                        }
                        return JsonConvert.SerializeObject(data);
                    }
                    catch (Exception ex) { return null; }
                }
                if (type == "upsert")
                {
                    try
                    {
                        var filter = JsonConvert.DeserializeObject(value.filter).ToString().Replace("\"", "'");
                        var filterDocument = BsonSerializer.Deserialize<BsonDocument>(filter);

                        var json = JsonConvert.DeserializeObject(value.para).ToString().Replace("\"", "'");
                        var document = BsonSerializer.Deserialize<BsonDocument>(json);

                        UpdateOptions op = new UpdateOptions();
                        op.IsUpsert = true;

                        var rs = await collection.UpdateManyAsync(filterDocument, document, op);
                        if (rs != null && rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null)
                            return "[]";
                    }
                    catch (Exception ex) { return null; }
                }

                if (type == "delete")
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject(value.filter).ToString().Replace("\"", "'");
                        var document = BsonSerializer.Deserialize<BsonDocument>(json);
                        collection.DeleteManyAsync(document).Wait();
                        return "[]";
                    }
                    catch (Exception ex) { return null; }
                }
                return null;
            }
            catch (Exception ex) { return null; }
        }
    }
}
