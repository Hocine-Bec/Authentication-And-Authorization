using BasicAuthentication.Authentication;
using BasicAuthentication.Data;
using BasicAuthentication.DTOs;
using BasicAuthentication.Entities;
using BasicAuthentication.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BasicAuthentication.Controllers
{
    [Route("Authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtOptions _jwtOptions;

        public AuthController(AppDbContext context, JwtOptions jwtOptions)
        {
            _context = context;
            _jwtOptions = jwtOptions;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> AuthenticateUserAsync(AuthUserRequest request)
        {
            if (request == null)
                return BadRequest("No user credentials were provided");

            var userCredentials = await GetAuthenticatedUserCredentials(request);

            if (userCredentials == null)
                return Unauthorized("Invalid Credentials");


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Lifetime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
                    SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, userCredentials.Id.ToString()),
                    new(ClaimTypes.Name, userCredentials.Username)
                })
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(token);
        }

        private async Task<User?> GetAuthenticatedUserCredentials(AuthUserRequest request)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.Password))
                return null;

            return user;
        }
    }
}
