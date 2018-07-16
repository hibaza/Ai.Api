using Ai.Data.Repositories.Mongo;
using Ai.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ai.Service
{
    public class ExtentionService
    {
        AppSettings _appSettings;
        ProceduceAiService _proceduceAiService;
        TemplatesRepository _templatesRepository;
        public ExtentionService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _proceduceAiService = new ProceduceAiService(appSettings);
            _templatesRepository = new TemplatesRepository(appSettings);
        }


        public async Task<List<BsonDocument>> ConvertMongoToBsonAsync(List<BsonDocument> lstBson, BsonDocument parentSession, BsonDocument quetionIntent, BsonDocument customerInfo)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                var newList = new List<BsonDocument>();
                BsonDocument newBson = new BsonDocument();
                var template = new BsonDocument();
                string business_id = "", template_id = "";
                if (lstBson != null && lstBson.Count > 0 && lstBson[0].Contains("template_id"))
                {
                    business_id = (string)lstBson[0]["business_id"];
                    template_id = (string)lstBson[0]["template_id"];
                    template = await _templatesRepository.GetById(business_id, template_id);
                }
                foreach (var bson in lstBson)
                {
                    try
                    {
                        //tasks.Add(Task.Factory.StartNew(() =>
                        //{
                        var intent = "product";
                        var strData = bson.ToString().ToLower();

                        if (quetionIntent != null)
                        {
                            intent = quetionIntent["quetionintent"].ToString();
                        }

                        newBson = new BsonDocument();
                        if (template.GetValue(intent, null) != null)
                        {
                            #region get title
                            try
                            {
                                var str = "";
                                var title = template[intent]["reply"]["title"];
                                if (title != null && title != "")
                                {
                                    var sp = title.ToString().Split(new string[] { "{{", "}}" }, StringSplitOptions.None);
                                    var i = 0;
                                    foreach (var r in sp)
                                    {
                                        if (i % 2 != 0)
                                        {
                                            var y = r.Split('.');
                                            if (bson[y[0]][y[1]] != "")
                                                str += bson[y[0]][y[1]];

                                        }
                                        else
                                        {
                                            if (sp[i] != "")
                                                str += sp[i];
                                        }
                                        i++;
                                    }
                                }
                                if (!newBson.Contains("messager"))
                                    newBson.Add("messager", str);
                                else
                                    newBson["messager"] = newBson["messager"].ToString() + " - &#13;&#10 " + str;
                            }
                            catch (Exception ex) { }
                            #endregion

                            #region add image
                            try
                            {
                                var imageUrl = "";
                                var image = template[intent]["reply"]["image"].ToString();
                                if (image != "")
                                {
                                    var sp = image.ToString().Split(new string[] { "{{", "}}" }, StringSplitOptions.None);
                                    var i = 0;
                                    foreach (var r in sp)
                                    {
                                        if (i % 2 != 0)
                                        {
                                            var y = r.Split('.');
                                            if (bson[y[0]][y[1]] != "")
                                                imageUrl = (bson[y[0]][y[1]].ToString());
                                        }
                                        i++;
                                    }
                                }
                                if (!newBson.Contains("image"))
                                    newBson.Add("image", imageUrl);
                                else
                                    newBson["image"] = newBson["image"].ToString() + " - &#13;&#10 " + imageUrl;
                            }
                            catch (Exception ex) { }
                            #endregion

                            #region add website link
                            try
                            {
                                if (!newBson.Contains("webdetailurl") && bson.Contains("websiteurl"))
                                {
                                    var website = template[intent]["reply"]["website"].ToString();
                                    if (website != "")
                                    {
                                        var sp = website.ToString().Split(new string[] { "{{", "}}" }, StringSplitOptions.None);
                                        var i = 0;
                                        foreach (var r in sp)
                                        {
                                            if (i % 2 != 0)
                                            {
                                                var y = r.Split('.');
                                                if (bson[y[0]][y[1]] != "")
                                                    newBson.Add("webdetailurl", bson[y[0]][y[1]].ToString());
                                            }
                                            i++;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { }
                            #endregion

                            if (!newBson.Contains("intent"))
                                newBson.Add("intent", parentSession != null ? parentSession["intent"] : "buy");
                            if (!newBson.Contains("last_message"))
                                newBson.Add("last_message", parentSession != null ? parentSession["last_message"] : "");
                            if (!newBson.Contains("using_full_search"))
                                newBson.Add("using_full_search", parentSession != null ? parentSession["defaultsearch"] : "1");
                            if (!newBson.Contains("currentsession"))
                                newBson.Add("currentsession", parentSession != null ? parentSession["product"] : "");
                            if (!newBson.Contains("reply_format"))
                                newBson.Add("reply_format", template[intent]["reply"]["format"]);
                            if (!newBson.Contains("product") && bson.Contains("product"))
                                newBson.Add("product", bson["product"]["value"]);
                            if (newBson.Count() > 0)
                            {
                                // thêm thay thông tin anh chị
                                try
                                {
                                    var str = newBson["messager"].ToString();
                                    if (str != "" && str.IndexOf(">>") > 0)
                                    {
                                        var sp = str.Split(new string[] { "<<", ">>" }, StringSplitOptions.None);
                                        var i = 0;
                                        var text = "";
                                        foreach (var r in sp)
                                        {
                                            if (i % 2 != 0)
                                            {
                                                if (r != "")
                                                    text += customerInfo != null ? customerInfo[r]=="male" ? "anh" : "chị" : customerInfo == null && r == "sex" ? "anh/chị" : "";
                                            }
                                            else
                                            {
                                                if (r != "")
                                                    text += r;
                                            }
                                            i++;
                                        }
                                        newBson["messager"] = text;
                                    }
                                    newList.Add(newBson);
                                }
                                catch (Exception ex) { }
                            }
                        }
                        else
                        {
                            #region them truong hop check ton kho
                            if (intent == "inventory")
                            {
                                try
                                {
                                    var url = _appSettings.BaseUrls.StockBalance;
                                    var dic = new Dictionary<string, string>();
                                    dic.Add("Id", bson["productid"]["value"].ToString());
                                    dic.Add("accesstoken", "bazavietnam2017");
                                    var para = JsonConvert.SerializeObject(dic);
                                    var client = new HttpClient();
                                    var response = client.PostAsync(url, new StringContent(para, Encoding.UTF8, "application/json")).Result;
                                    var contents = response.Content.ReadAsStringAsync().Result;
                                    var obj = JsonConvert.DeserializeObject<dynamic>(contents);

                                    newBson.Add("messager", bson["attributes"]["desc"].ToString() + " - " +
                                        (string)obj.StockInfo);

                                    newBson.Add("image", bson["imageurl"]["value"].ToString());

                                    if (!newBson.Contains("intent"))
                                        newBson.Add("intent", parentSession != null ? parentSession["intent"] : "buy");
                                    if (!newBson.Contains("last_message"))
                                        newBson.Add("last_message", parentSession != null ? parentSession["last_message"] : "");
                                    if (!newBson.Contains("using_full_search"))
                                        newBson.Add("using_full_search", parentSession != null ? parentSession["defaultsearch"] : "1");
                                    if (!newBson.Contains("currentsession"))
                                        newBson.Add("currentsession", parentSession != null ? parentSession["product"] : "");
                                    if (!newBson.Contains("reply_format"))
                                        newBson.Add("reply_format", "text");
                                    if (!newBson.Contains("product") && bson.Contains("product"))
                                        newBson.Add("product", bson["product"]["value"]);
                                    if (!newBson.Contains("webdetailurl"))
                                        newBson.Add("webdetailurl", "");
                                    newList.Add(newBson);
                                }
                                catch { }
                            }

                            #endregion
                        }
                        //}));
                    }
                    catch (Exception ex)
                    {
                    }
                    //Task.WaitAll(tasks.ToArray());

                }
                var t = newList.GroupBy(i => i["messager"], (key, group) => group.First()).ToArray().Distinct().ToList().GroupBy(i => i["messager"], (key, group) => group.First()).Distinct().ToList();

                return t;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<BsonDocument>> addFunctionAsync(List<BsonDocument> lstBson)
        {
            try
            {
                //var t = Task<List<BsonDocument>>.Factory.StartNew(() =>
                //{
                foreach (var bson in lstBson)
                {
                    var messager = bson["messager"].ToString();
                    var sum = "";
                    if (messager != "" && messager.IndexOf("[[") > 0)
                    {
                        var sp = messager.Split(new string[] { "[[", "]]" }, StringSplitOptions.None);
                        var para = "";
                        var func = "";

                        for (var i = 0; i < sp.Length; i++)
                        {
                            if (i % 2 != 0)
                            {
                                var strs = sp[i].Split(',');
                                for (var j = 0; j < strs.Length; j++)
                                    if (j >= 1)
                                        para += "'" + strs[j] + "',";
                                    else
                                        func = strs[j];

                                var cmd = func + "(" + para.Substring(0, para.Length - 1) + ")";                                
                                var sessionResult = await _proceduceAiService.Proceduce(cmd);//.RunCommandAsync<BsonDocument>(cmd).Result;
                                var entities = sessionResult["retval"];
                                if (entities.ToString().Length > 5)
                                {
                                    sum += entities["message"];
                                }
                            }
                            else
                                sum += sp[i];
                        }
                    }
                    bson["messager"] = sum == "" ? messager : sum;
                }
                return lstBson;
                //});
                //return await t;
            }
            catch (Exception ex) { return lstBson; }
        }

        public async Task<string> ConvertOrderToJsonAsync(string data, string last_message, string intent, string session_customer)
        {
            try
            {
                //var t = Task<string>.Factory.StartNew(() =>
                //{
                data = data.Substring(0, data.Length - 1);
                string add = ",\"intent\":\"" + intent + "\"," +
                    "\"last_message\":\"" + last_message + "\"," +
                    "\"using_full_search\":\"0\"," +
                    "\"currentsession\":\"" + session_customer + "\"," +
                    "\"reply_format\":\"" + intent + "\"," +
                "\"product\":\"\"";

                data = data + add + "}";

                return data;
                //});
                //return await t;
            }
            catch (Exception ex) { return null; }
        }

    }
}
