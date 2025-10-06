///AppDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogApp.Models;

namespace BlogApp.Entity
{ 
    public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId });


        builder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<PostTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

    //     // Seed roles
    //     var adminRoleId = "b1a0d1e4-8c3f-4f3d-9a77-111111111111";
    //     var userRoleId = "c2b1e2f5-9d4e-5f4d-8b88-222222222222";
    //     var roles = new List<IdentityRole>
    //     {
    //         new IdentityRole
    //         {
    //             Id = adminRoleId,
    //             ConcurrencyStamp = adminRoleId,
    //             Name = "Admin",
    //             NormalizedName = "ADMIN"
    //         },
    //         new IdentityRole
    //         {
    //             Id = userRoleId,
    //             ConcurrencyStamp = userRoleId,
    //             Name = "User",
    //             NormalizedName = "USER"
    //         }
    //     };

    //     builder.Entity<IdentityRole>().HasData(roles);
    }
}

}  
