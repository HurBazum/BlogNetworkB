using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogNetworkB.Models.Account;

namespace BlogNetworkB.Controllers
{
    public class ArticleController : Controller
    {
        readonly IArticleRepository _articleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly ITagRepository _tagRepository;
        readonly ICommentRepository _commentRepository;
        readonly IMapper _mapper;
        public ArticleController(IArticleRepository articleRepository, IAuthorRepository authorRepository, ICommentRepository commentRepository, ITagRepository tagRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult WriteArticle() => View();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PublicArticle([FromForm] ArticleViewModel articleViewModel)
        {
            if(ModelState.IsValid)
            {
                var article = _mapper.Map<Article>(articleViewModel);

                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                article.AuthorId = author.Id;

                article.Author = author;

                article.CreatedDate = DateTime.Now;

                await _articleRepository.AddArticle(article);

                var model = _mapper.Map<AuthorViewModel>(author);
                model.ArticlesCount = _articleRepository.GetArticlesByAuthor(author).Result.Length;
                model.CommentsCount = _commentRepository.GetCommentByAuthor(author).Result.Length;

                return View("/Views/Author/MyPage.cshtml", model);
            }

            return View("WriteArticle", articleViewModel);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyArticlesList(int? id)
        {
            Author author = new();

            if (id == null)
            {
                author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);
            }
            if(id != null)
            {
                author = await _authorRepository.GetAuthorById((int)id);
            }

            var articles = await _articleRepository.GetArticlesByAuthor(author);
            
            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            return View(new ArticleListViewModel { Articles = avmArray });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ArticleList()
        {
            var articles = await _articleRepository.GetAll();

            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            return View("MyArticleList", new ArticleListViewModel { Articles = avmArray });
        }
    }
}
