using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ai.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ai.Core;
using MongoDB.Bson;
using Ai.Service;
using Microsoft.Extensions.Options;
using Ai.Core.TokenProviders;
using Ai.Domain;
using Newtonsoft.Json;

namespace Ai.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/AiExcuteAll")]
    public class AiExcuteAllController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        AiService _aiService;

        public AiExcuteAllController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _aiService = new AiService(appSettings.Value);
        }
        //var config = {
        //session_customer: _customer_id,
        //    receive_type: receiveType,
        //    using_full_search: $(".defaultSearch").prop('checked') == true ? "1" : "0",
        //    page_id: channel_ext_id,
        //    auto_agents: autoAgents,
        //    page_next_bot: _pageNextBot,
        //    business_id: businessID
        //};
        //var para = {
        //    q: last_message == null || last_message == undefined ? "" : last_message.replace("'", " ").replace("\\", " ").replace("\n", " ").replace("/", " ").replace("&#13;&#10", " ").replace("\"", " ")
        //};
        // POST: api/AiExcuteAll
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("Excute")]
        [AllowAnonymous]
        public async Task<string> Excute([FromBody] mongodb value)
        {
            var config = Core.Helpers.CommonHelper.JsonToBsonDocument(value.config);
            var para = Core.Helpers.CommonHelper.JsonToBsonDocument(value.para);
            var session_customer = !config.Contains("session_customer")?"":(string)config["session_customer"];
            var q = (string)para["q"];
            var page_next_bot = !config.Contains("page_next_bot") ? 0 : (int)config["page_next_bot"];
            var receive_type = !config.Contains("receive_type") ?"": (string)config["receive_type"];            
            var using_full_search = config.Contains("using_full_search") && (string)config["using_full_search"] == "1"?1:0;
           
            var page_id = !config.Contains("page_id") ? "" : (string)config["page_id"];
            var auto_agents = !config.Contains("auto_agents") ? "" : (string)config["auto_agents"];
            var business_id = !config.Contains("business_id") ? "" : (string)config["business_id"];

            var key = "data_" + session_customer + "_" +
                Core.Helpers.CommonHelper.removeSpecialString(q) + "_" + page_next_bot;


            if (!string.IsNullOrWhiteSpace(auto_agents) && auto_agents != "manualagents" && q != "")
            {
                var checkCache = CacheBase.GetItem(key);
                if (checkCache != null)
                    return (string)checkCache;
            }
            return await _aiService.GetProduct(q, session_customer, receive_type, using_full_search, page_id,
                auto_agents, page_next_bot, business_id,key);

        }
    }
}
