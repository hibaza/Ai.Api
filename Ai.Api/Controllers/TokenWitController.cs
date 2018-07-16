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
using Ai.Service.Tool.Hand;

namespace Ai.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/token-wit")]
    public class TokenWitController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        WitTokenService _witTokenService;

        public TokenWitController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _witTokenService = new WitTokenService(appSettings.Value);
        }
        
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("get-by-businessid")]
        [AllowAnonymous]
        public async Task<string> GetTokenWit([FromBody] DynamicPara obj)
        {
            var para = Core.Helpers.CommonHelper.JsonToBsonDocument(obj.para);
            var rs =  await _witTokenService.GetById((string)para["business_id"]);
            return Core.Helpers.CommonHelper.BsonToJson(rs);
        }

    }
}
