using Microsoft.AspNetCore.Identity;

namespace BlogApp.Repository
{
    public interface ITokenRepository
    {
        string CreateToken(ApplicationUser user);
    }
}