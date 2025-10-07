// Models/PostTag.cs
namespace BlogApp.Models
{
    public class PostTag
    {
    public int PostId { get; set; }   // FK -> Posts.Id
    public Post Post { get; set; }

    public int TagId { get; set; }    // FK -> Tags.Id
    public Tag Tag { get; set; }

    // Nếu cần thêm dữ liệu trên quan hệ: 
    // public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}

