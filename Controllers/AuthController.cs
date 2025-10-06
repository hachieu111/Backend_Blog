using BlogApi.models.Repository;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        //POST: api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var identityUser = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerDTO.Password); //create user -> add to DB, pass to hashpass

            if (identityResult.Succeeded) // create succeed
            {
                if (registerDTO.Roles != null && registerDTO.Roles.Any()) // role not null && role in DTO any of them
                {
                    await userManager.AddToRolesAsync(identityUser, registerDTO.Roles); // add role
                    if (identityResult.Succeeded)
                    {
                        return Ok("User registered successfully."); //200 200
                    }
                }
            }
            return BadRequest(identityResult.Errors); 
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
                    var jwtToken = tokenRepository.CreateToken(user);

                    return Ok(jwtToken);
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

        public string[] Roles { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}