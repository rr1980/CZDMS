using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CZDMS.Db;
using CZDMS.Db.Entities;
using CZDMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CZDMS.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly FileDbContext _fileDbContext;

        public TokenController(IConfiguration config, FileDbContext fileDbContext)
        {
            _configuration = config;
            _fileDbContext = fileDbContext;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userLoginData">User Login-Data</param>
        /// <returns>Acess-Tokenr</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">ModelState invalid</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post(UserLoginModel userLoginData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(userLoginData.Username, userLoginData.Password);

            if (user != null)
            {
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("UserName", user.Username)
                   };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                var response = new JwtSecurityTokenHandler().WriteToken(token);

                Debug.WriteLine("Token: " + response);

                return Ok(response);
            }
            else
            {
                return BadRequest("Invalid credentials");
            }
        }

        private async Task<User> GetUser(string username, string password)
        {
            return await _fileDbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}
