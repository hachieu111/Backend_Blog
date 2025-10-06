// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using BlogApp.Models;
using BlogApp.Entity;
public class ApplicationUser : IdentityUser
{
   public ICollection<Post> Posts { get; set; } = new List<Post>();
}
