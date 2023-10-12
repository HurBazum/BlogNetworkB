namespace BlogNetworkB.Models.Comment
{
    public class CommentViewModel : CreateCommentViewModel
    {
        public int CommentId { get; set; }
        public string ArticleName { get; set; }
        public string AuthorName { get; set; }
    }
}