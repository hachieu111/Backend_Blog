// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    
    public string FullName { get; set; }
   public ICollection<Post> Posts { get; set; } = new List<Post>();
}
