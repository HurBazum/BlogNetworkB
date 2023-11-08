using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Tag;

namespace BlogNetworkB.BLL.Services.Interfaces
{
    public interface IArticleService
    {
        public Task<ArticleDTO[]> GetAllArticleDTOs();
        public Task<ArticleDTO[]> GetArticleDTOsByAuthor(AuthorDTO authorDTO);
        public Task AddArticle(ArticleDTO articleDTO);
        public Task<TagDTO[]> GetArticleTagsDTOs(ArticleDTO articleDTO);
        public Task<ArticleDTO> GetArticleDTOById(int id);
        public Task UpdateArticle(ArticleDTO articleDTO, UpdateArticleRequest uar);
        public Task DeleteArticle(int id);
    }
}