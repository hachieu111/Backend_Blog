using BlogApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApp.Controllers
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

        // Only Admin can assign roles
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound($"Không tìm thấy user với email {dto.Email}");

            // // Nếu role chưa có -> tạo mới
            // if (!await roleManager.RoleExistsAsync(dto.Role))
            // {
            //     var newRole = new IdentityRole(dto.Role);
            //     await _roleManager.CreateAsync(newRole);
            // }

            // Gán role cho user
            var result = await userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Đã gán role '{dto.Role}' cho user '{dto.Email}'");
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

    public class AssignRoleDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }
}