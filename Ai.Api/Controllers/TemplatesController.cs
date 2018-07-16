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
using Ai.Service.Tool;

namespace Ai.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Templates")]
    public class TemplatesController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        TemplatesService _templatesService;

        public TemplatesController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _templatesService = new TemplatesService(appSettings.Value);
        }
        
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("GetList")]
        [AllowAnonymous]
        public async Task<string> GetList([FromBody] DynamicPara obj)
        {
            var para = Core.Helpers.CommonHelper.JsonToBsonDocument(obj.para);
            return await _templatesService.GetList((string)para["business_id"]);
        }

    }
}
