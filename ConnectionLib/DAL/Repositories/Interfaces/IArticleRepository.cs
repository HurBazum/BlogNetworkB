using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Article;

namespace ConnectionLib.DAL.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        public Task AddArticle(Article article);
        public Task UpdateArticle(Article article, UpdateArticleQuery updateArticleQuery);
        public Task DeleteArticle(Article article);
        public Task<Article> GetArticleById(int id);
        public Task<Article[]> GetArticleByName(string name);
        public Task<Article[]> GetAll();
        public Task AddTag(Article article, Tag tag);
        public Task<Article[]> GetArticlesByAuthor(Author author);
        public Task<Tag[]> GetArticlesTags(Article article);
    }
}