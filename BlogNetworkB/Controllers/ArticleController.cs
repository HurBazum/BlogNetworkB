using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Queries.Article;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogNetworkB.Models.Account;
using BlogNetworkB.Models.Tag;

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
        [Route("/[controller]/NewArticle")]
        public async Task<IActionResult> WriteArticle()
        {
            var avm = new ArticleViewModel();
            var tags = _tagRepository.GetAll().Result.Select(tag => tag.Content).ToList();
            
            foreach (var tag in tags)
            {
                avm.ArticleTags.Add(tag, false);
            }

            return View(avm);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/NewArticle")]
        public async Task<IActionResult> PublicArticle([FromForm] ArticleViewModel articleViewModel)
        {
            if(ModelState.IsValid)
            {
                var article = _mapper.Map<Article>(articleViewModel);

                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                article.AuthorId = author.Id;

                article.Author = author;

                article.CreatedDate = DateTime.Now;

                //foreach(var tag in articleViewModel.ArticleTags)
                //{
                //    article.Tags.Add(_mapper.Map<Tag>(tag));
                //}

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
        [Route("/[controller]/Author/Articles")]
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
        [Route("/[controller]/AllArticles")]
        public async Task<IActionResult> ArticleList()
        {
            var articles = await _articleRepository.GetAll();

            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            return View("MyArticlesList", new ArticleListViewModel { Articles = avmArray });
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/ReadArticle")]
        public async Task<IActionResult> ReadArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            var avm = _mapper.Map<ArticleViewModel>(article);

            return View(avm);
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/RewriteArticle")]
        public async Task<IActionResult> RewriteArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            UpdateArticleRequestViewModel uarvm = new() { ArticleId = article.Id, NewTitle = article.Title, NewContent = article.Content };

            return View(uarvm);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/RewriteArticle")]
        public async Task<IActionResult> ConfirmRewriteArticle([FromForm]UpdateArticleRequestViewModel model)
        {
            if(ModelState.IsValid)
            {
                var article = await _articleRepository.GetArticleById(model.ArticleId);
                
                // для редиректа
                var currentAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                var uar = _mapper.Map<UpdateArticleRequest>(model);

                await _articleRepository.UpdateArticle(article, _mapper.Map<UpdateArticleQuery>(uar));

                return RedirectToAction("ArticleList");
            }

            return View("RewriteArticle", model.ArticleId);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteArticle")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            await _articleRepository.DeleteArticle(article);

            return RedirectToAction("ArticleList");
        }
    }
}
