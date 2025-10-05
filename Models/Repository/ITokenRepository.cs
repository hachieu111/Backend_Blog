using Microsoft.AspNetCore.Identity;

namespace BlogApi.models.Repository
{
    public interface ITokenRepository
    {
        string CreateToken(ApplicationUser user);
    }
}