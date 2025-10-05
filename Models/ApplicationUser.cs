// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    
    //public string UserName { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
