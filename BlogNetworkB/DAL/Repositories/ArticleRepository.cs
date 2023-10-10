using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Queries.Article;
using BlogNetworkB.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogNetworkB.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        readonly BlogContext _blogContext;
        public ArticleRepository(BlogContext blogContext) => _blogContext = blogContext;

        #region добавление, изменение, удаление
        public async Task AddArticle(Article article)
        {
            var entry = _blogContext.Articles.Entry(article);
            if (entry.State == EntityState.Detached)
            {
                await _blogContext.AddAsync(article);
                await _blogContext.SaveChangesAsync();
            }
        }

        public async Task DeleteArticle(Article article)
        {
            var entry = _blogContext.Articles.Entry(article);
            entry.State = EntityState.Deleted;
            await _blogContext.SaveChangesAsync();
        }

        public async Task UpdateArticle(Article article, UpdateArticleQuery updateArticleQuery)
        {
            article = ArticleConverter.Convert(article, updateArticleQuery);
            var entry = _blogContext.Articles.Entry(article);
            entry.State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }

        public async Task AddTag(Article article, Tag tag)
        {
            article.Tags.Add(tag);
            var entry = _blogContext.Articles.Entry(article);
            entry.State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }
        #endregion

        #region методы получения данных из бд
        public async Task<Article[]> GetAll() => await _blogContext.Articles.ToArrayAsync();
        public async Task<Article> GetArticleById(int id) => await _blogContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
        public async Task<Article[]> GetArticleByName(string name) => await _blogContext.Articles.Where(a => a.Title == name).ToArrayAsync();
        public async Task<Article[]> GetArticlesByAuthor(Author author) => await _blogContext.Articles.Where(a => a.AuthorId == author.Id).ToArrayAsync();
        public async Task<Tag[]> GetArticlesTags(Article article) => await _blogContext.Articles.Where(a => a.Id == article.Id).SelectMany(a => a.Tags).ToArrayAsync();
        #endregion
    }
}