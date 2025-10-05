using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]

public class PostController : ControllerBase
{
    [Authorize]
    [HttpGet("secure")]
    public IActionResult GetSecurePosts()
    {
        return Ok(new { message = "Bạn đã vào được endpoint bảo vệ rồi!" });
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult GetPublicPosts()
    {
        return Ok(new { message = "Ai cũng xem được endpoint này." });
    }
}
