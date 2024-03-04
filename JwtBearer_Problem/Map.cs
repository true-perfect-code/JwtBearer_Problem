using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtBearer_Problem
{
    public static class SymKey
    {
        //public static string SymmetricSecKey = "11111222222@eeeeeWWW"; // !!! Test implementation !!!
        public static string SymmetricSecKey = "b693a53f-ad24-4fb0-bca0-92f56ae6406b"; // !!! Test implementation !!!
    }
    

    public class UserWebApi
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public static class Map
    {
        public static void MapEndPoints(this WebApplication app)
        {
            app.MapPost("/security/token", (UserWebApi user) => Token(user)).AllowAnonymous();

            app.MapPost("/api/loginstatus", () => Status()).RequireAuthorization();
        }

        /// <summary>
        /// Generates a token
        /// </summary>
        /// <param name="_user">Model object contains all information from the client</param>
        /// <returns>Return value is token</returns>
        public static async Task<IResult> Token(UserWebApi _user)
        {
            if (_user.Email == "admin" && _user.Password == "123") // !!! Test implementation !!!
            {
                var claims = new[] {
                                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                        new Claim(JwtRegisteredClaimNames.Email, "admin"), // Email
                                        new Claim(JwtRegisteredClaimNames.GivenName, "admin") // User_ID
                                    };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SymKey.SymmetricSecKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    //issuer: url,
                    //audience: url,
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return await Task.FromResult(Results.Ok(new { user = "admin", token = tokenString }));
            }
            else
                return Results.Unauthorized();
        }

        /// <summary>
        /// Returns the status of the WebApi connection
        /// </summary>
        /// <param name="_user">Model object contains all information from the client</param>
        /// <returns>Return value depending on the token validation</returns>
        public static async Task<string> Status()
        {
            return await Task.FromResult("You are logged in!");
        }


    }
}

