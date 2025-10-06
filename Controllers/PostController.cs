// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using BlogApp.Models;
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

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/posts
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    AuthorName = p.Author != null ? p.Author.UserName : "Unknown",
                    p.PublishedDate
                })
                .ToListAsync();

            return Ok(posts);
        }

        // GET: /api/posts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    AuthorName = p.Author != null ? p.Author.UserName : "Unknown",
                    Tags = p.PostTags.Select(pt => new { pt.Tag.Id, pt.Tag.Name }).ToList(),
                    p.PublishedDate
                })
                .FirstOrDefaultAsync();

            if (post == null) return NotFound();
            return Ok(post);
        }

        // POST: /api/posts
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                AuthorId = userId,
                PublishedDate = DateTime.UtcNow
            };

            post.PostTags = new List<PostTag>();

            // Thêm tag (tạo mới nếu chưa có)
            foreach (var tagName in dto.TagNames.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync(); // để có Id
                }

                post.PostTags.Add(new PostTag { Post = post, Tag = tag });
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, new { post.Id, post.Title });
        }

        // PUT: /api/posts/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var post = await _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            if (post.AuthorId != userId)
                return Forbid();

            // Cập nhật nội dung
            post.Title = dto.Title ?? post.Title;
            post.Content = dto.Content ?? post.Content;

            // Cập nhật tags
            post.PostTags.Clear();
            foreach (var tagName in dto.TagNames.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                post.PostTags.Add(new PostTag { Post = post, Tag = tag });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/posts/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
                return NotFound();

            if (post.AuthorId != userId)
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTOs
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> TagNames { get; set; } = new();
    }

    public class UpdatePostDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> TagNames { get; set; } = new();
    }
}
