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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Ai.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/agents")]
    public class AgentsController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IOptions<AppSettings> _appSettings;
        AgentsService _agentsService;

        public AgentsController(IOptions<JwtIssuerOptions> jwtOptions, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _jwtOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            _agentsService = new AgentsService(appSettings.Value);
        }

        [EnableCors("SiteCorsPolicy")]
        [HttpGet("auth/{username}/{password}")]
        [AllowAnonymous]
        public async Task<IActionResult> Token(string username, string password)
        {
            var json = "";
            var agent = await _agentsService.GetAgentByUserPass(username, password);
            if (agent == null)
                return null;

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub,agent["username"].ToString()),
        new Claim(ClaimTypes.Role, agent["role"].ToString()),
        new Claim("user_id", agent["id"].ToString()),
        new Claim("name",agent["name"].ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat,TokenProviderMiddleware.ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
                };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                user_id = agent["id"].ToString(),
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
                role = (string)agent["role"],
                business_id = (string)agent["business_id"],
            };
            json = JsonConvert.SerializeObject(response, _serializerSettings);

            return new OkObjectResult(json);
        }

    }
}
