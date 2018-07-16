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
    [Route("api/ai-suggestions")]
    public class AiSuggestionsController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        AiSuggestionsService _suggestionsService;

        public AiSuggestionsController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _suggestionsService = new AiSuggestionsService(appSettings.Value);
        }
        
        [EnableCors("SiteCorsPolicy")]
        [HttpGet("get-by-business/{business_id}")]
        [AllowAnonymous]
        public async Task<string> GetByBusiness(string business_id)
        {
           return  await _suggestionsService.GetByBusiness(business_id);           
        }

    }
}
