using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        //POST: api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var identityUser = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                //UserName = registerDTO.UserName,
                Email = registerDTO.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerDTO.Password);

            if (identityResult.Succeeded)
            {
                return Ok("User registered successfully.");
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }
        }


        // POST api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (checkPasswordResult)
                {
                    // Generate JWT token

                    return Ok("Successfully logged in.");
                }
            }
            return BadRequest("Invalid login attempt.");
        }
    }

    //DTO class
    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}