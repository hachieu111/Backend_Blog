using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=LAPTOP-TKD6P9RP\\SQLEXPRESS;Database=Blog;User Id=sa;Password=123;TrustServerCertificate=True;");

        return new AppDbContext(optionsBuilder.Options);
    }
}
