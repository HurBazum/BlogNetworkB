namespace BlogNetworkB.Models.Comment
{
    public class CommentViewModel : CreateCommentViewModel
    {

        public string Article { get; set; }
        public string Author { get; set; }
    }
}