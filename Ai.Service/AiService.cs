
using Ai.Data.Repositories.Mongo;
using Ai.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MongoDB.Driver;

namespace Ai.Service
{
    public class AiService
    {
        AppSettings _appSettings;
        WitTokenService _witTokenService;
        WitService _witService;
        ProceduceAiService _proceduceAiService;
        TemplatesRepository _templatesRepository;
        ProductsRepository _productsRepository;
        ExtentionService _extentionService;
        ProductCaresService _productCaresService;
        ProductGeneralRepository _productGeneralRepository;
        public AiService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _witTokenService = new WitTokenService(appSettings);
            _witService = new WitService(appSettings);
            _proceduceAiService = new ProceduceAiService(appSettings);
            _templatesRepository = new TemplatesRepository(appSettings);
            _productsRepository = new ProductsRepository(appSettings);
            _extentionService = new ExtentionService(appSettings);
            _productCaresService = new ProductCaresService(appSettings);
            _productGeneralRepository = new ProductGeneralRepository(appSettings);
        }

        
        public async Task<string> GetProduct(string q, string  session_customer, string receive_type, int using_full_search, string channel_id,
              string auto_agents,int page_next_bot, string business_id,string key)
        {
            var paras = Core.Helpers.CommonHelper.CutStringMax(q, 200);
            
            foreach (var strShort in paras)
            {
                if (strShort != "")
                {
                    var witTokenData = await _witTokenService.GetById(business_id);

                    var witResult = await GetDataFromWit(q, witTokenData);

                    // lấy ra 1 số thông số của wit để kết nối database

                    var dicWit = await _witService.FormatWitAsync(witResult);
                    var witEntities = dicWit["fomart"].Replace("\r\n","");
                    var entitiesString = dicWit["entities"];
                    var data = "";
                    if (witEntities.Length > 5)
                    {
                        data = await GetDataFromMongoHaveWit(strShort, witEntities, entitiesString, dicWit,
                            session_customer, receive_type, using_full_search, channel_id, auto_agents, page_next_bot, business_id);
                    }
                    else
                    {
                        if (using_full_search > 0)
                        {
                            data = await GetDataFromMongoNoWit(strShort, witEntities, session_customer,  receive_type, 
                                 using_full_search,  channel_id, auto_agents,  page_next_bot,  business_id);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        CacheBase.SetItem(key, data, DateTime.UtcNow.AddMinutes(10));
                    }
                    return data;
                }
            }


            return "";
        }

        public async Task<string> GetDataFromMongoHaveWit(string strShort, string witEntities, string entitiesString, Dictionary<string, string> dicWit,
             string session_customer, string receive_type, int using_full_search, string channel_id,
              string auto_agents, int page_next_bot, string business_id)
        {
            List<BsonDocument> results = new List<BsonDocument>();

            var witDes = JsonConvert.DeserializeObject(witEntities);
            //  var cmd1 = new JsonCommand<BsonDocument>("{ eval: \"jsParentSession({'x': '2','y': '4'})\"}");

            var cmd = "\"jsParentSession('" + session_customer + "',"
                        + witDes.ToString().Replace("\"", "'") + ",'" + strShort + "',[" + entitiesString.Substring(0, entitiesString.Length - 1) + "]," +
                       using_full_search + ",'" + (!string.IsNullOrWhiteSpace(auto_agents) ? "" : auto_agents.ToLower()) + "')\"";

            var sessionResult = await _proceduceAiService.Proceduce(cmd);
            // sau khi có dữ liệu sẽ gọi tiếp sang database mongodb để lấy thông tin shop
            if (sessionResult != null && sessionResult["retval"] != null && sessionResult["retval"].ToString().Length > 5)
            {
                var entities = sessionResult["retval"][0];
                if (entities.ToString().Length > 0)
                {
                    var parentSs = sessionResult["retval"][1] != null && sessionResult["retval"][1].ToString().Length > 10 ? (BsonDocument)sessionResult["retval"][1] : null;
                    var quetionIntent = sessionResult["retval"][2] != null && sessionResult["retval"][2].ToString().Length > 10 ? (BsonDocument)sessionResult["retval"][2] : null;
                    var customerInfo =  sessionResult["retval"][3] != null && sessionResult["retval"][3].ToString().Length > 10 ? (BsonDocument)sessionResult["retval"][3] : null;

                    #region tìm bên mongodb để lấy thông tin 
                    var query = await _witService.formatCommandAsync(entities.ToString());
                    if (query.Length > 10 && query.IndexOf("parentproduct.value") >= 0)
                    {
                        if (query.Length > 10)
                        {
                            var data = await _productsRepository.GetParameterDynamic(query);
                            var rs = await _extentionService.ConvertMongoToBsonAsync(data, parentSs, quetionIntent, customerInfo);
                            results.AddRange(rs);

                            // them luu khach hang dang quan tam san pham nao
                            await _productCaresService.Upsert(dicWit, session_customer, business_id, channel_id);
                            #endregion
                        }
                        // tìm thêm intent chung chung như địa chỉ công ty... bảo hành
                        query = await _witService.formatCommandAsync(witEntities.ToString());
                        if (query.Length > 10)
                        {

                            var generalData = await _productGeneralRepository.GetParameterDynamic(query);
                            var rs1 = await _extentionService.ConvertMongoToBsonAsync(generalData, parentSs, quetionIntent, customerInfo);
                            results.AddRange(rs1);
                        }
                        // sử lý những trường hợp đặc biệt , lấy giá trị qua funfion
                        results = await _extentionService.addFunctionAsync(results);
                        if (results.Count > 0)
                        {
                            var d = results.ToJson();
                            //CacheBase.AddItem(key, d, DateTime.Now.AddMinutes(10), session_customer);
                            return d;
                        }
                    }
                    else
                    {
                        if (using_full_search>0)
                        {
                            // full text search. nếu wit không có giá trị
                            query = "";
                            query = await _witService.formatCommandAsync(witEntities.ToString());

                            var search = "";
                            if (query.Length > 10)
                                search = query.Substring(0, query.Length - 3) + ",{$text: {$search: \"" + strShort + "\"}}]}";
                            else
                                search = "{$text: {$search: \"" + strShort + "\"}}";

                            var productData = await _productsRepository.GetFullTextSearch(search, page_next_bot);

                            var rs = await _extentionService.ConvertMongoToBsonAsync(productData, null, null, null);
                            results.AddRange(rs);

                            results = await _extentionService.addFunctionAsync(results);
                            if (results.Count > 0)
                            {
                                var d = results.ToJson();
                                return d;
                            }
                        }
                    }
                }
            }
            return "[]";
        }

        public async Task<string> GetDataFromMongoNoWit(string strShort, string witEntities, string session_customer, string receive_type, int using_full_search, string channel_id,
              string auto_agents, int page_next_bot, string business_id)
        {
            List<BsonDocument> results = new List<BsonDocument>();

            // full text search. nếu wit không có giá trị va dang chon su dung full text search
            var query = await _witService.formatCommandAsync(witEntities);

            var search = "";
            if (query.Length > 10)
                // search = "{$text: {$search: \"" + strShort + "\"}," + query.Substring(1, query.Length - 2) + "}";
                search = query.Substring(0, query.Length - 3) + ",{$text: {$search: \"" + strShort + "\"}}]}";
            else
                search = "{$text: {$search: \"" + strShort + "\"}}";

            var productData = await _productsRepository.GetFullTextSearch(search, page_next_bot);
            var rs = await _extentionService.ConvertMongoToBsonAsync(productData, null, null, null);
            results.AddRange(rs);

            results = await _extentionService.addFunctionAsync(results);
            if (results.Count > 0)
            {
                var d = results.ToJson();
               // CacheBase.AddItem(key, d, DateTime.Now.AddMinutes(10), configDic["session_customer"]);
                return d;
            }
            return "[]";
        }

        public async Task<string> GetDataFromWit(string q, BsonDocument witTokenData)
        {
            var str = "v="+DateTime.Now.ToString("yyyyMMdd")+"&q=" + q.Trim().ToLower() + "&access_token=" + witTokenData["wittoken"];
            return await Core.Helpers.WebHelper.GetAsync(_appSettings.BaseUrls.AiMessagerLink + str);
        }
    }
}
