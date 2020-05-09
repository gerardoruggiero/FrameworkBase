using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace Framework.Security.Token
{
    public static class TokenGenerator
    {
        public static string GenerateTokenJwt(string username)
        {
            // appsetting for Token JWT
            var secretKey = TokenValidationHandler.JWT_SECRET_KEY;
            var audienceToken = TokenValidationHandler.JWT_AUDIENCE_TOKEN;
            var issuerToken = TokenValidationHandler.JWT_ISSUER_TOKEN;
            var expireTime = TokenValidationHandler.JWT_EXPIRE_MINUTES;

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }
    }
}