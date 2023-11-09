using BlogNetworkB.BLL.Models.Tag;
using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Article
{
    public class CreateArticleO
    {
        [Required]
        [MinLength(5, ErrorMessage = "Минимальная длина заголовка - 5 символов")]
        [MaxLength(40, ErrorMessage = "Максимальная длина заголовка - 40 символов")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Минимальная длина статьи - 15 символов")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public ICollection<TagDTO> ArticleTagDTOs { get; set; } = new List<TagDTO>();
    }
}