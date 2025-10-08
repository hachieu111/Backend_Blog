using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogApp.Entity
{
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=Server=DESKTOP-URE4FO1\\SQLEXPRESS;Database=Blog;User Id=sa;Password=12345;TrustServerCertificate=True;");

        return new AppDbContext(optionsBuilder.Options);
    }
}
}
