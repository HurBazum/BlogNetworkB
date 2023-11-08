using AutoMapper;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Tag;
using BlogNetworkB.BLL.Services.Interfaces;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Article;
using ConnectionLib.DAL.Repositories.Interfaces;

namespace BlogNetworkB.BLL.Services
{
    public class ArticleService : IArticleService
    {
        readonly IArticleRepository _articleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly ITagRepository _tagRepository;
        readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, IAuthorRepository authorRepository, ITagRepository tagRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<ArticleDTO[]> GetArticleDTOsByAuthor(AuthorDTO authorDTO)
        {
            var author = await _authorRepository.GetAuthorById(authorDTO.AuthorId);
            var articles = await _articleRepository.GetArticlesByAuthor(author);

            return _mapper.Map<ArticleDTO[]>(articles);
        }

        public async Task AddArticle(ArticleDTO articleDTO)
        {
            var article = _mapper.Map<Article>(articleDTO);

            foreach(var dto in articleDTO.ArticleTagDTOs)
            {
                var tag = await _tagRepository.GetTagById(dto.TagId);
                article.Tags.Add(tag);
            }

            article.CreatedDate = DateTime.UtcNow;

            await _articleRepository.AddArticle(article);
        }

        public async Task<TagDTO[]> GetArticleTagsDTOs(ArticleDTO articleDTO)
        {
            var article = await _articleRepository.GetArticleById(articleDTO.ArticleId);

            var tags = await _articleRepository.GetArticlesTags(article);

            return _mapper.Map<TagDTO[]>(tags);
        }

        public async Task UpdateArticle(ArticleDTO articleDTO, UpdateArticleRequest uar)
        {
            var article = await _articleRepository.GetArticleById(articleDTO.ArticleId);

            await _articleRepository.UpdateArticle(article, _mapper.Map<UpdateArticleQuery>(uar));
        }

        public async Task DeleteArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);
            await _articleRepository.DeleteArticle(article);
        }

        public async Task<ArticleDTO[]> GetAllArticleDTOs()
        {
            var articles = await _articleRepository.GetAll();

            return _mapper.Map<ArticleDTO[]>(articles);
        }
        public async Task<ArticleDTO> GetArticleDTOById(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            return _mapper.Map<ArticleDTO>(article);
        }
    }
}