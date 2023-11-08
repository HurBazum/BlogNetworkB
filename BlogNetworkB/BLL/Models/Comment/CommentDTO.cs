namespace BlogNetworkB.BLL.Models.Comment
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ArticleId { get; set; }
        public int AuthorId { get; set; }
    }
}