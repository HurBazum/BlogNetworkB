using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Article
{
    public class UpdateArticleRequestViewModel
    {
        public int ArticleId { get; set; }


        [MinLength(5, ErrorMessage = "Минимальная длина заголовка - 5 символов")]
        [MaxLength(40, ErrorMessage = "Максимальная длина заголовка - 40 символов")]
        [DataType(DataType.Text)]
        public string NewTitle { get; set; }


        [MinLength(15, ErrorMessage = "Минимальная длина статьи - 15 символов")]
        [DataType(DataType.MultilineText)]
        public string NewContent { get; set; }
    }
}
