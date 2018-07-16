using Ai.Domain;
using MongoDB.Bson;
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
    public class WitService
    {

        AppSettings _appSettings;
        public WitService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<Dictionary<string, string>> FormatWitAsync(string witResult)
        {
            var d = Task<Dictionary<string, string>>.Factory.StartNew(() =>
            {
                var dic = new Dictionary<string, string>();
                var str = "[";
                var entitiesString = "";
                dynamic obj = JObject.Parse(witResult);
                foreach (JProperty en in obj)
                {
                    try
                    {
                        if (en.Name.ToLower().Equals("entities"))
                        {
                            foreach (JProperty val in en.Value)
                            {
                                foreach (var e in val.Value)
                                {
                                    decimal confidence = 0.0M;

                                    foreach (JProperty j in e)
                                    {
                                        // nếu kết quả thông thường
                                        if (!j.Name.ToLower().Equals("entities"))
                                        {
                                            if (e.ToString().IndexOf("entities") < 0)
                                            {
                                                if (j.Name.ToLower().Equals("confidence"))
                                                    confidence = Convert.ToDecimal(j.Value);
                                                if (j.Name.ToLower().Equals("value") && confidence >= _appSettings.Confidence)
                                                {
                                                    if (val.Value.Count() == 1)
                                                    {
                                                        var v = "{" + val.ToString() + "}";
                                                        if (str.IndexOf(v) < 0)
                                                        {
                                                            str += v + ",";
                                                            entitiesString += "'" + val.Name + "',";
                                                        }
                                                    }
                                                    if (val.Value.Count() > 1)
                                                    {
                                                        if (val.Value.First.ToString() == val.Value.Last.ToString())
                                                        {
                                                            var v = "{\"" + val.Name.ToString() + "\":[" + e.ToString() + "]}";
                                                            if (str.IndexOf(v) < 0)
                                                            {
                                                                str += v + ",";
                                                                entitiesString += "'" + val.Name + "',";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var v = "{" + val.ToString() + "}";
                                                            if (str.IndexOf(v) < 0)
                                                            {
                                                                str += v + ",";
                                                                entitiesString += "'" + val.Name + "',";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (JProperty h in j.Value)
                                            {
                                                foreach (var g in h.Value)
                                                {
                                                    foreach (JProperty t in g)
                                                    {
                                                        if (t.Name.ToLower().Equals("confidence"))
                                                            confidence = Convert.ToDecimal(t.Value);
                                                        if (t.Name.ToLower().Equals("value") && confidence >=_appSettings.Confidence)
                                                        {
                                                            var v = "{\"" + val.Name.ToString() + "\":[" + g.ToString() + "]}";
                                                            if (str.IndexOf(v) < 0)
                                                            {
                                                                str += v + ",";
                                                                entitiesString += "'" + val.Name + "',";
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
                str += "]";
                dic.Add("fomart", str);
                dic.Add("entities", entitiesString);
                return dic;
            });
            return await d;
        }

        //public async Task<Dictionary<string, string>> FormatWitAsync(string witResult, Dictionary<string, string> webSetting)
        //{
        //    var d = Task<Dictionary<string, string>>.Factory.StartNew(() =>
        //     {
        //         var dic = new Dictionary<string, string>();
        //         var str = "[";
        //         var entitiesString = "";
        //         dynamic obj = JObject.Parse(witResult);
        //         foreach (JProperty en in obj)
        //         {
        //             try
        //             {
        //                 foreach (var k in (JArray)en.Value)
        //                 {
        //                     if (k["__wit__legacy_response"] !=null && k["__wit__legacy_response"]["entities"] !=null)
        //                     {
        //                         foreach (JProperty val in k["__wit__legacy_response"]["entities"])
        //                         {
        //                             foreach (var e in val.Value)
        //                             {
        //                                 decimal confidence = 0.0M;

        //                                 foreach (JProperty j in e)
        //                                 {
        //                                     // nếu kết quả thông thường
        //                                     if (!j.Name.ToLower().Equals("entities"))
        //                                     {
        //                                         if (e.ToString().IndexOf("entities") < 0)
        //                                         {
        //                                             if (j.Name.ToLower().Equals("confidence"))
        //                                                 confidence = Convert.ToDecimal(j.Value);
        //                                             if (j.Name.ToLower().Equals("value") && confidence >= Convert.ToDecimal(webSetting["confidence"]))
        //                                             {
        //                                                 if (val.Value.Count() == 1)
        //                                                 {
        //                                                     var v = "{" + val.ToString() + "}";
        //                                                     if (str.IndexOf(v) < 0)
        //                                                     {
        //                                                         str += v + ",";
        //                                                         entitiesString += "'" + val.Name + "',";
        //                                                     }
        //                                                 }
        //                                                 if (val.Value.Count() > 1)
        //                                                 {
        //                                                     if (val.Value.First.ToString() == val.Value.Last.ToString())
        //                                                     {
        //                                                         var v = "{\"" + val.Name.ToString() + "\":[" + e.ToString() + "]}";
        //                                                         if (str.IndexOf(v) < 0)
        //                                                         {
        //                                                             str += v + ",";
        //                                                             entitiesString += "'" + val.Name + "',";
        //                                                         }
        //                                                     }
        //                                                     else
        //                                                     {
        //                                                         var v = "{" + val.ToString() + "}";
        //                                                         if (str.IndexOf(v) < 0)
        //                                                         {
        //                                                             str += v + ",";
        //                                                             entitiesString += "'" + val.Name + "',";
        //                                                         }
        //                                                     }
        //                                                 }
        //                                             }
        //                                         }
        //                                     }
        //                                     else
        //                                     {
        //                                         foreach (JProperty h in j.Value)
        //                                         {
        //                                             foreach (var g in h.Value)
        //                                             {
        //                                                 foreach (JProperty t in g)
        //                                                 {
        //                                                     if (t.Name.ToLower().Equals("confidence"))
        //                                                         confidence = Convert.ToDecimal(t.Value);
        //                                                     if (t.Name.ToLower().Equals("value") && confidence >= Convert.ToDecimal(webSetting["confidence"]))
        //                                                     {
        //                                                         var v = "{\"" + val.Name.ToString() + "\":[" + g.ToString() + "]}";
        //                                                         if (str.IndexOf(v) < 0)
        //                                                         {
        //                                                             str += v + ",";
        //                                                             entitiesString += "'" + val.Name + "',";
        //                                                         }
        //                                                         break;
        //                                                     }
        //                                                 }
        //                                             }
        //                                         }
        //                                     }
        //                                 }
        //                             }
        //                         }
        //                     }
        //                 }
        //             }
        //             catch { }
        //         }
        //         str += "]";
        //         dic.Add("fomart", str);
        //         dic.Add("entities", entitiesString);
        //         return dic;
        //     });
        //    return await d;
        //}
        public async Task<string> formatCommandAsync(string entities)
        {
            if (entities.Length < 5)
                return "";
            var query = "{$and:[";
            string and = "", or = "";
            var dt = new DataTable("dt");
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("entity"), new DataColumn("value") });

            foreach (var r in JArray.Parse(entities))
            {
                try
                {
                    foreach (var j in (JObject)r)
                    {
                        foreach (var h in j.Value)
                        {
                            dt.Rows.Add(j.Key.ToString().ToLower(), h.ToString().ToLower());
                        }
                    }
                }
                catch { }
            }
            var dr = dt.Select("", "entity ASC");
            var newDt = dt.Copy();// dr.CopyToDataTable();
            var different = newDt.Rows[0]["entity"].ToString();
            var i = 0;
            var m = 0;
            foreach (DataRow row in newDt.Rows)
            {
                try
                {
                    var des = JsonConvert.DeserializeObject<dynamic>(row["value"].ToString());

                    var en = row["entity"].ToString();
                    var val = (string)des.value;
                    var count = newDt.Select("entity = '" + en + "'").Count();

                    if (en == "intent")
                    {
                        // lấy ra intent
                        if (val != "buy" && val != "order" && val != "checkout")
                        {
                            var t = "{'" + val + "':{ $exists: true }},";
                            if (and.IndexOf(t) < 0)
                                and += t;
                        }
                    }
                    else
                    {
                        // lấy ra query                            
                        if (count <= 1)
                        {
                            // nếu wit trả ra 1 thì toàn bộ là and
                            if (en == "product")
                            {
                                // produc tìm theo kiểu like
                                //var t = "{'" + en + ".value':/" + val + "/},";
                                var t = "{'parentproduct.value':'" + val + "'},";
                                if (and.IndexOf(t) < 0)
                                    and += t;
                            }
                            else
                            // loại khác thì tìm chính xác
                            {
                                var t = "{'" + en + ".value':" + (!(await IsNumericAsync(val)) ? "'" + val + "'" : val) + "},";
                                if (and.IndexOf(t) < 0)
                                    and += t;
                            }
                        }
                        else
                        {
                            if (en == "product")
                            {
                                // produc tìm theo like
                                //var t = "{'" + en + ".value':/" + val + "/},";
                                var t = "{'parentproduct.value':'" + val + "'},";
                                if (or.IndexOf(t) < 0)
                                    or += t;
                            }
                            else
                            {
                                if (await IsNumericAsync(val))
                                // trong câu có so sánh trong khoảng thì vào đây ví dụ giá tiền trong khoảng 2 tr đến 3 triệu
                                {
                                    var t = "{'" + en + ".value':" + (m == 0 ? "{$gte:" : "{$lte:") + val + "}},";
                                    if (and.IndexOf(t) < 0)
                                    {
                                        and += t;
                                        m++;
                                    }
                                }
                                else
                                {
                                    // tìm theo điều kiện or ví dụ nhiều màu , xanh , vàng
                                    var t = "{'" + en + ".value':'" + val + "'},";
                                    if (or.IndexOf(t) < 0)
                                        or += t;
                                }
                            }
                        }


                    }
                    different = i < newDt.Rows.Count - 1 ? newDt.Rows[(i + 1)]["entity"].ToString() : "";
                    if (different != en)
                    {
                        if (or != "")
                        {
                            and += "{$or:[" + or + "]},";
                            or = "";
                        }

                    }
                    i++;
                }
                catch { }
            }
            return query = query + and + or + "]}";

        }

        public async Task<bool> IsNumericAsync(string str)
        {
            var d = Task<bool>.Factory.StartNew(() =>
            {
                NumberStyles style = NumberStyles.Number;
                CultureInfo culture = null;
                double num;
                if (culture == null) culture = CultureInfo.InvariantCulture;
                return Double.TryParse(str, style, culture, out num) && !String.IsNullOrWhiteSpace(str);
            });
            return await d;
        }

    }
}
