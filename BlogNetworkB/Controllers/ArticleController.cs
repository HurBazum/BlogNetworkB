using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Article;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogNetworkB.Models.Account;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc.Filters;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Article")]
    public class ArticleController : Controller
    {
        readonly IArticleRepository _articleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly ITagRepository _tagRepository;
        readonly ICommentRepository _commentRepository;
        readonly IMapper _mapper;
        readonly ILogger<ArticleController> _logger;
        public ArticleController(IArticleRepository articleRepository, IAuthorRepository authorRepository, ICommentRepository commentRepository, ITagRepository tagRepository, IMapper mapper, ILogger<ArticleController> logger)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/NewArticle")]
        public IActionResult WriteArticle(ArticleViewModel? avm)
        {
            try
            {
                if (avm == null)
                {
                    avm = new ArticleViewModel();
                }

                var tags = _tagRepository.GetAll().Result.Select(tag => tag.Content).ToList();

                if (!tags.Any())
                {
                    throw new CustomException("Попытка добавить статью. Нет доступных тегов. Сначала добавьте хотя бы один тег на странице с \'Теги\'");
                }

                foreach (var tag in tags)
                {
                    if (!avm.ArticleTags.Contains(tag))
                    {
                        avm.ArticleTags.Add(tag);
                    }
                }

                _logger.LogInformation("Пользователь {email} приступил к созданию статьи", HttpContext.User.Claims.FirstOrDefault().Value);

                return View(avm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }


        /// <summary>
        /// пока можно добавлять статьи не указывая теги
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("/[controller]/NewArticle")]
        public async Task<IActionResult> PublicArticle([FromForm]ArticleViewModel articleViewModel, params string[] tags)
        {
            if(ModelState.IsValid)
            {
                var article = _mapper.Map<Article>(articleViewModel);

                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                article.AuthorId = author.Id;

                article.Author = author;

                article.CreatedDate = DateTime.UtcNow;

                if(tags.Length == 0)
                {
                    ModelState.AddModelError("ArticleTags", "Необходимо указать хотя бы один тег");

                    _logger.LogWarning("Не были указаны теги");

                    tags = _tagRepository.GetAll().Result.Select(tag => tag.Content).ToArray();

                    articleViewModel.ArticleTags = tags;

                    return View("WriteArticle", articleViewModel);
                }

                foreach(var t in tags)
                {
                    var tag = await _tagRepository.GetTagByContent(t);
                    article.Tags.Add(tag);
                }

                try
                {
                    await _articleRepository.AddArticle(article);

                    _logger.LogInformation("Статья '{article}' была создана пользователем {email}", article.Title, author.Email);
                }
                catch(Exception ex)
                {
                    _logger.LogError("Пользователь {email} не смог добавить статью", author.Email);
                    CustomErrorViewModel cevm = new() { Message = string.Concat(ex.Message, "\n", $"Пользователь {author.Email} не смог добавить статью") };
                    return View("/Views/Alert/SomethingWrong.cshtml");
                }

                var model = _mapper.Map<AuthorViewModel>(author);
                model.ArticlesCount = _articleRepository.GetArticlesByAuthor(author).Result.Length;
                model.CommentsCount = _commentRepository.GetCommentByAuthor(author).Result.Length;

                return View("/Views/Author/MyPage.cshtml", model);
            }

            _logger.LogWarning("Неверно заполнена форма");

            //// чтоб было возможно выбрать из всех тегов, а не только
            //// тех, что были переданны в этот метод
            var tagsName = _tagRepository.GetAll().Result.Select(tag => tag.Content).ToList();

            //articleViewModel.ArticleTags = tagsName;
            foreach (var tag in tagsName)
            {
                articleViewModel.ArticleTags.Add(tag);
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

                _logger.LogInformation("Пользователь {email} перешёл к списку своих статей", author.Email);
            }
            if(id != null)
            {
                author = await _authorRepository.GetAuthorById((int)id);

                _logger.LogInformation("Пользователь {currEmail} перешёл к списку статей автора {email}", HttpContext.User.Claims.FirstOrDefault().Value, author.Email);
            }

            var articles = await _articleRepository.GetArticlesByAuthor(author);
            
            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            for(int i = 0; i < articles.Length; i++)
            {
                avmArray[i].ArticleTags = _articleRepository.GetArticlesTags(articles[i]).Result.Select(t  => t.Content).ToList();
            }

            foreach (var article in avmArray)
            {
                article.AuthorEmail = author.Email;
            }

            return View(new ArticleListViewModel { Articles = avmArray });
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/AllArticles")]
        public async Task<IActionResult> ArticleList()
        {
            var articles = await _articleRepository.GetAll();

            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            for(int i = 0; i < articles.Length; i++)
            {
                avmArray[i].ArticleTags = _articleRepository.GetArticlesTags(articles[i]).Result.Select(t => t.Content).ToList();
            }

            _logger.LogInformation("Пользователь {email} перешёл к списку всех статей", HttpContext.User.Claims.FirstOrDefault().Value);

            return View("MyArticlesList", new ArticleListViewModel { Articles = avmArray });
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/ReadArticle")]
        public async Task<IActionResult> ReadArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            var author = await _authorRepository.GetAuthorById(article.AuthorId);

            var avm = _mapper.Map<ArticleViewModel>(article);

            var comments = await _commentRepository.GetCommentByArticle(article);

            foreach(var comment in comments.ToList())
            {
                var cvm = _mapper.Map<CommentViewModel>(comment);
                cvm.AuthorName = _authorRepository.GetAuthorById(comment.AuthorId).Result.Email;
                cvm.ArticleName = article.Title;
                avm.ArticleComments.Comments.Add(cvm);
            }

            avm.AuthorEmail = author.Email;

            _logger.LogInformation("Пользователь {email} прочитал статью {article}", HttpContext.User.Claims.FirstOrDefault().Value, avm.Title);

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
                //var currentAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                var uar = _mapper.Map<UpdateArticleRequest>(model);

                await _articleRepository.UpdateArticle(article, _mapper.Map<UpdateArticleQuery>(uar));

                _logger.LogInformation("Пользователь {email} изменил {articleId} статью", HttpContext.User.Claims.FirstOrDefault().Value, article.Id);

                return RedirectToAction("ArticleList");
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("RewriteArticle", model.ArticleId);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteArticle")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _articleRepository.GetArticleById(id);

            await _articleRepository.DeleteArticle(article);

            _logger.LogInformation("Пользователем {email} была удалена статья {article}", HttpContext.User.Claims.FirstOrDefault().Value, article.Id);

            return RedirectToAction("ArticleList");
        }
    }
}