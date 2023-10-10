namespace BlogNetworkB.Models.Comment
{
    public class CreateCommentViewModel
    {
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; init; }

        public CreateCommentViewModel()
        {
            CreatedDate = DateTime.Now;
        }
    }
}