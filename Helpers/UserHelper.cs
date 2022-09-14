﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TentaPApi.Models;
using TentaPApi.RequestBodies;
using WebApiUtilities.Helpers;

namespace TentaPApi.Helpers
{
    public class UserHelper
    {
        public static UserLoginRequestBody AuthenticateUser(UserLoginRequestBody loginInformation, User user)
        {
            if (PasswordHelper.ValidPassword(loginInformation.Password, user.Password))
                return new UserLoginRequestBody() { Email = loginInformation.Email, Password = loginInformation.Password };
            return null;
        }

        public static string GenerateJsonWebToken(User user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvironmentHelper.GetEnvironmentVariable("JWT_KEY")));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
            };

            JwtSecurityToken token = new JwtSecurityToken("sakur.se", "sakur.se", claims, expires: DateTime.Now.AddDays(14), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static Dictionary<string, string> GetClaims(ClaimsPrincipal user)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (Claim claim in user.Claims)
            {
                if (claim.Properties.Count > 0)
                {
                    foreach (string propertyKey in claim.Properties.Keys)
                    {
                        result.Add(claim.Properties[propertyKey], claim.Value);
                    }
                }
            }

            return result;
        }
    }
}
