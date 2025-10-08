using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApi.Migrations;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        // GET: /api/posts/by-tag/{tagName}
        [HttpGet("/api/posts/by-tag/{tagName}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByTag(string tagName)
        {
            var posts = await _context.Posts
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Where(p => p.PostTags.Any(pt => pt.Tag.Name == tagName))
                .ToListAsync();

            if (!posts.Any())
            {
                return NotFound($"Không tìm thấy bài viết nào với tag '{tagName}'.");
            }

            return Ok(posts);
        }
    }
}