using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokyoBike.Models;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context, ApplicationContext appCtx)
        {
            var token = context.Request.Headers["Token"].FirstOrDefault()?.Split(" ").Last();

            if(token != null)
            {
                AuthorizeToken(context, appCtx, token);
            }

            await next(context);
        }

        public void AuthorizeToken(HttpContext context, ApplicationContext appCtx, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                // min 16 characters

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE
                }, out SecurityToken validatedToken) ;

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                context.Items["User"] = appCtx.Users.Include(u => u.Statistics).Include(s => s.Statistics.Days).Include(s => s.Statistics.Rates).FirstOrDefault(u => u.Id == userId);
            }
            catch(Exception ex)
            {
                context.Response.WriteAsync(ex.Message);
                // todo: need to add logger
            }
        }
    }
}
