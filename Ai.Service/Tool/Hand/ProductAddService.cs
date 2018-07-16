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

namespace Ai.Service.Tool.Hand
{
    public class ProductAddService
    {

        AppSettings _appSettings;
        ProductsRepository _productsRepository;

        public ProductAddService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _productsRepository = new ProductsRepository(appSettings);
        }
        public async Task<string> SearchProduct(string business_id, string product)
        {
            var rs = await _productsRepository.SearchProduct(business_id, product);
            return Core.Helpers.CommonHelper.BsonToJson(rs);
        }

        public async Task<string> SearchProducts(string business_id, string product)
        {
            var rs = await _productsRepository.SearchProducts(business_id, product);
            return rs.ToJson();
        }

        public async Task<bool> Upsert(BsonDocument data)
        {
            var rs= await _productsRepository.upsert(data);
            if (rs.MatchedCount > 0 || rs.IsModifiedCountAvailable || rs.ModifiedCount > 0 )
                return true;
            else
                return false;
        }
    }
}
