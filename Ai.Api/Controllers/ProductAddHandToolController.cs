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
    [Route("api/ProductAddHandTool")]
    public class ProductAddHandToolController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        ProductAddService _productAddService;

        public ProductAddHandToolController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _productAddService = new ProductAddService(appSettings.Value);
        }
        
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("SearchProduct")]
        [AllowAnonymous]
        public async Task<string> SearchProduct([FromBody] DynamicPara obj)
        {
            var para = Core.Helpers.CommonHelper.JsonToBsonDocument(obj.para);
            return await _productAddService.SearchProduct((string)para["business_id"], (string)para["product"]);
        }

        [EnableCors("SiteCorsPolicy")]
        [HttpPost("SearchProducts")]
        [AllowAnonymous]
        public async Task<string> SearchProducts([FromBody] DynamicPara obj)
        {
            var para = Core.Helpers.CommonHelper.JsonToBsonDocument(obj.para);
            return await _productAddService.SearchProducts((string)para["business_id"], (string)para["product"]);
        }

        [EnableCors("SiteCorsPolicy")]
        [HttpPost("AddProduct/{business_id}/{id}/{template_id}")]
        [AllowAnonymous]
        public async Task<bool> AddProduct(string business_id,string id, string template_id, [FromBody] DynamicPara obj)
        {
            try
            {
                if (business_id + "_" == id|| business_id + "_product" == id)
                    return false;

                var data = Core.Helpers.CommonHelper.JsonToBsonDocument(obj.para);
                
               return await _productAddService.Upsert(data);
                
            }
            catch(Exception ex) { return false; }
        }
    }
}
