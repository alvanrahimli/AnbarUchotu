using System.Linq;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Repos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnbarUchotu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _repo;

        public AuthController(IAuthRepo repo)
        {
            this._repo = repo;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserLoginDto loginCreds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.Login(loginCreds);

            if (result.userCredentials != null)
            {
                return Ok(new
                {
                    token = result.token,
                    userCredentials = result.userCredentials
                });
            }
            return Unauthorized();
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserRegisterDto newUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.Register(newUser);

            if (result.userCredentials != null)
            {
                return Ok(new
                {
                    token = result.token,
                    userCredentials = result.userCredentials
                });
            }

            return BadRequest("Username or Email is registered already.");
        }


        [Authorize]
        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }
    }
}