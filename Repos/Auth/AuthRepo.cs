using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AnbarUchotu.Data;
using AnbarUchotu.Models;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AnbarUchotu.Repos.Auth
{
    public class AuthRepo : IAuthRepo
    {
        private readonly AnbarUchotuDbContext _context;
        private readonly IConfiguration _config;

        public AuthRepo(AnbarUchotuDbContext context, IConfiguration config)
        {
            this._context = context;
            this._config = config;
        }
        public async Task<(UserReturnDto userCredentials, string token)> Login(UserLoginDto loginCreds)
        {
            UserReturnDto loggedUser = await AuthecticateUser(loginCreds);

            if (loggedUser != null)
            {
                string token = GenerateToken(loggedUser);
                return (loggedUser, token);
            }

            return (null, null);
        }

        private string GenerateToken(UserReturnDto userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Guid),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"]),
                new Claim(type: "roles", value: userInfo.Role ?? "Buyer")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        private async Task<UserReturnDto> AuthecticateUser(UserLoginDto loginCreds)
        {
            var pass = Helper.ComputeHash(loginCreds.Password);
            var usr = await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginCreds.Email.ToLower());

            if (usr != null)
            {
                bool isValid = Helper.Equals(usr.PasswordHash, pass);
                if (isValid)
                {
                    UserReturnDto userReturnDto = new UserReturnDto()
                    {
                        Guid = usr.Guid,
                        Username = usr.Username,
                        Email = usr.Email,
                        Role = usr.Role ?? "Buyer"
                    };
                    return userReturnDto;
                }
            }
            return null;
        }

        public async Task<(UserReturnDto userCredentials, string token)> Register(UserRegisterDto userCreds)
        {
            var emailExists = await EmailExists(userCreds.Email);

            if (!emailExists)
            {
                User newUser = new User()
                {
                    Guid = Guid.NewGuid().ToString(),
                    Email = userCreds.Email,
                    NormalizedEmail = userCreds.Email.ToLower(),
                    Username = userCreds.Username,
                    NormalizedUsername = userCreds.Username.ToLower(),
                    PasswordHash = Helper.ComputeHash(userCreds.Password)
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var loginResult = await Login(new UserLoginDto()
                {
                    Email = userCreds.Email,
                    Password = userCreds.Password
                });

                return loginResult;
            }
            return (null, null);
        }

        private async Task<bool> EmailExists(string email)
        {
            var usr = await _context.Users
                .AnyAsync(u => u.NormalizedEmail == email.ToLower());
            return usr;
        }
    }
}