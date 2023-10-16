namespace ConnectionLib.DAL.Enteties
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        // rel
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}