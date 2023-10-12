using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Comment
{
    public class UpdateCommentRequestViewModel
    {
        public int CommentId { get; set; }


        [MinLength(15, ErrorMessage = "Минимальная длина комментария - 10 символов")]
        [DataType(DataType.MultilineText)]
        public string NewContent { get; set; }
    }
}