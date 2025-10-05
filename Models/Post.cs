// Models/Post.cs
using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;

    public string AuthorId { get; set; }              
    public ApplicationUser Author { get; set; }      
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
