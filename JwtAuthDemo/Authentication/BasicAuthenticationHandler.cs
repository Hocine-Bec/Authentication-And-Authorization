using JwtAuthDemo.Data;
using JwtAuthDemo.Entities;
using JwtAuthDemo.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace JwtAuthDemo.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppDbContext _context;

        public BasicAuthenticationHandler(AppDbContext context, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Step 1: Check if there is a header named Authorization
            //Step 2: Check if the request header contains scheme Basic
            //Step 3: The date will be Base64Encoded, So decode it and extract from it username and password
            //Step 4: Check if the username and password are valid
            //Step 5: Prepare the Identity using Claims, Principal, and Ticket 
            //Step 6: Return Success with the ticket

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();

            var authHeader = Request.Headers["Authorization"].ToString();

            if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unknown Scheme");

            string encodedCredentials = authHeader["Basic ".Length..];
            string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var userCredentials = decodedCredentials.Split(":");

            var user = await GetUsernameAndPasswordAsync(userCredentials[0]);

            if (!IsUserAuthenticated(userCredentials, user))
                return AuthenticateResult.Fail("Invalid Username Or Password");

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }, "Basic");

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Basic");

            return AuthenticateResult.Success(ticket);
        }

        private async Task<User?> GetUsernameAndPasswordAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return (user != null) ? user : null;
        }

        private bool IsUserAuthenticated(string[] userCredentials, User? user)
        {
            if (user == null)
                return false;

            if (userCredentials[0] != user.Username || 
                !PasswordHelper.VerifyPassword(userCredentials[1], user.Password))
            {
                return false;
            }
            return true;
        }
    }
}
