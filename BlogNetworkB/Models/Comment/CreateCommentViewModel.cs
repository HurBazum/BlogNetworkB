using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Comment
{
    public class CreateCommentViewModel
    {
        public int AuthorId { get; set; }
        public int ArticleId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Слишком короткий комментарий")]
        [Display(Name = "Текст комментария")]
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; init; }

        public CreateCommentViewModel()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}