﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleEcommerceV2.IdentityServer.Domain.Services.Contracts;
using SimpleEcommerceV2.Shared.InOut.Requests;

namespace SimpleEcommerceV2.IdentityServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AuthenticateAsync
        (
            [FromServices] IConfiguration configuration,
            [FromServices] IUserService userService,
            [FromBody] AuthRequest authRequest
        )
        {
            var isAuthenticated = await userService.CheckPasswordAsync(authRequest);
            if (!isAuthenticated)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Identity:Key")!));
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Email, authRequest.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Identity:TokenLifetime")),
                Issuer = configuration.GetValue<string>("Identity:Issuer"),
                Audience = configuration.GetValue<string>("Identity:Audience"),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            if (!authRequest.OnlyTokenBody)
                return Ok(new
                {
                    token = jwt,
                    expiration = token.ValidTo
                });

            return Ok(jwt);
        }
    }
}
