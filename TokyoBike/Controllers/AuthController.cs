using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TokyoBike.Models;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Controllers
{
    //[ApiController]
    [Route("api/[controller]")]   
    
    public class AuthController : Controller
    {
        ApplicationContext appCtx;
        public AuthController(ApplicationContext context)
        {
            appCtx = context;
        }

        [Route("googleAuth")]
        public IActionResult GoogleAuth()
        {            
            var propertis = new AuthenticationProperties { RedirectUri = "/api/auth/google-response" };

            return Challenge(propertis, GoogleDefaults.AuthenticationScheme);
        }
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var responce =  await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var result = responce.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value                
            }).ToArray();

            User user = new User
            {
                Login = result[1].Value,
                Email = result[result.Length - 1].Value
            };
            
            if(appCtx.Users.FirstOrDefault(u => u.Email == user.Email) == null)
            {
                AddUser(user.Login, null, user.Email);
            }
            HttpContext.Items["User"] = appCtx.Users.FirstOrDefault(u => u.Email == user.Email);

            return Authenticate(new UserModel { Email = user.Email, Password = null});
        }

        [Route("google-logout")]
        public async Task<IActionResult> GoogleLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var result = HttpContext.User.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            return Json(result);
        }

        public User AddUser(string login, string password, string email)
        {
            if (appCtx.Users.FirstOrDefault(u => u.Email == email) == null)
            {
                //Validation
                User user = new User { Email = email, Password = password, Role = "user", Login = login };

                appCtx.Users.Add(user);
                appCtx.SaveChanges();
                var u = appCtx.Users.ToList().Last();
                u.Statistics = new Statistics { UserId = u.Id };
                appCtx.Entry(u).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                appCtx.SaveChanges();

                return user;
            }
            return null;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel us)        
        {
            User u = AddUser(us.Login, us.Password, us.Email);
            if (u != null)
            {
                return Authenticate(new UserModel { Email = u.Email, Password = u.Password});
            }
            return BadRequest(new { errormesage = "This email is already used" });
        }

        [HttpPost("authenticate")]       
        public IActionResult Authenticate([FromBody]UserModel us)
        {
            User user = appCtx.Users.FirstOrDefault(x => x.Email == us.Email && x.Password == us.Password);
            var identity = GetIdentity(user);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);           

            return Json(new AuthResponse(user, encodedJwt));
        }

        private ClaimsIdentity GetIdentity(User u)
        {
            
            if (u != null)
            {
                var claims = new List<Claim>
                {                    
                    new Claim(ClaimsIdentity.DefaultNameClaimType, u.Email),
                    new Claim("id", u.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, u.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

    }
}
