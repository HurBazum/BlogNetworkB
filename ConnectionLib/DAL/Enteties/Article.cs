namespace ConnectionLib.DAL.Enteties
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        // rel
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}