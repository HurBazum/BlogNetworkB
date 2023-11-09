using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Comment
{
    public class CreateCommentO
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Слишком короткий комментарий")]
        public string Content { get; set; }
        public int ArticleId { get; set; }
        public int AuthorId { get; set; }
    }
}
