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

namespace Ai.Service
{
    public class ProductCaresService
    {

        AppSettings _appSettings;
        ProductCaresRepository _productCaresRepository;

        public ProductCaresService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _productCaresRepository = new ProductCaresRepository(appSettings);
        }
        public async Task<ReplaceOneResult> Upsert(Dictionary<string, string> dicWit, string session_customer, string business_id, string channel_id)
        {
            try
            {
                if (dicWit != null && dicWit.Count > 0 && dicWit.ContainsKey("fomart"))
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(dicWit["fomart"]);
                    var id = business_id + "_" + channel_id + "_" + session_customer + "_" + (string)obj[0].product[0].value;

                    var para = new BsonDocument();
                    para.Add("product", (string)obj[0].product[0].value);
                    para.Add("session_customer", session_customer);
                    para.Add("quantily", 1);
                    para.Add("timestamp", Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow));
                    para.Add("created_time", DateTime.UtcNow);
                    para.Add("_id", id);
                    para.Add("id", id);
                    para.Add("business_id", business_id);
                    para.Add("channel_id", channel_id);
                    return await _productCaresRepository.Upsert(para);
                }
                return null;
            }
            catch (Exception ex) { return null; }
        }
    }
}
