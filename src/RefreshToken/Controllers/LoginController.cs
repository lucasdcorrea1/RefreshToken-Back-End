using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Whodo.Autentication.Models;
using Whodo.Autentication.Repositories;
using Whodo.Autentication.Services;

namespace RefreshToken.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.Name, model.Password);
            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos." });

            var token = TokenService.GenerateToken(user);
            var refreshToken = TokenService.GenerateRefrashToken();
            TokenService.SaveRefrashToken(user.Name, refreshToken);

            user.Password = string.Empty;

            return new
            {
                user = user,
                token = token,
                refreshToken = refreshToken
            };
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<dynamic>> Refresh(string token, string refreshToken)
        {
            var principal = TokenService.GetPrincipalFromExpuredToken(token);
            var username = principal.Identity.Name;
            var saveRefreshToken = TokenService.GetRefreshToken(username);
            if (saveRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = TokenService.GenerateToken(principal.Claims);

            var newRefreshToken = TokenService.GetRefreshToken(username);
            TokenService.DeleRefreshToken(username, refreshToken);
            TokenService.SaveRefrashToken(username, newRefreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken

            });
        }

    }
}
