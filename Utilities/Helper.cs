using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AnbarUchotu.Utilities
{
    public class Helper
    {
        public static byte[] ComputeHash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] passHash = sha256Hash
                    .ComputeHash(Encoding.UTF8.GetBytes(password));
                return passHash;
            }
        }

        public static bool Equals(byte[] pass, byte[] input)
        {
            for (int i = 0; i < pass.Length; i++)
            {
                if (pass[i] != input[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetCurrentUserGuid(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claims = identity.Claims.ToList();
            return claims[0].Value;
        }
    }
}