using BlogNetworkB.BLL.Models.Tag;

namespace BlogNetworkB.BLL.Models.Article
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AuthorId { get; set; }
        public ICollection<TagDTO> ArticleTagDTOs { get; set; } = new List<TagDTO>();
    }
}