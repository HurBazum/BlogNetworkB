using AutoMapper;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogNetworkB.Models.Account;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc.Filters;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Models.Author;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Article")]
    public class ArticleController : Controller
    {
        readonly IMapper _mapper;
        readonly ILogger<ArticleController> _logger;
        readonly IArticleService _articleService;
        readonly IAuthorService _authorService;
        readonly ICommentService _commentService;
        readonly ITagService _tagService;
        public ArticleController(
            IMapper mapper,
            ILogger<ArticleController> logger,
            IAuthorService authorService, 
            IArticleService articleService,
            ICommentService commentService,
            ITagService tagService)
        {
            _mapper = mapper;
            _logger = logger;
            _authorService = authorService;
            _articleService = articleService;
            _commentService = commentService;
            _tagService = tagService;
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

                var tags = _tagService.GetAllTagsDTOs().Result.Select(dto => dto.Content).ToList();

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
                return View("/Views/Alert/NotFound.cshtml", cevm);
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
                var article = _mapper.Map<ArticleDTO>(articleViewModel);

                var author = await _authorService.GetAuthorDTOByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                article.AuthorId = author.AuthorId;

                article.CreatedDate = DateTime.UtcNow;

                if(tags.Length == 0)
                {
                    ModelState.AddModelError("ArticleTags", "Необходимо указать хотя бы один тег");

                    _logger.LogWarning("Не были указаны теги");

                    tags = _tagService.GetAllTagsDTOs().Result.Select(dto => dto.Content).ToArray();

                    articleViewModel.ArticleTags = tags;

                    return View("WriteArticle", articleViewModel);
                }

                article.ArticleTagDTOs = await _tagService.GetTagsDTOsByContent(tags);

                try
                {
                    await _articleService.AddArticle(article);

                    _logger.LogInformation("Статья '{article}' была создана пользователем {email}", article.Title, author.Email);
                }
                catch(Exception ex)
                {
                    _logger.LogError("Пользователь {email} не смог добавить статью", author.Email);
                    CustomErrorViewModel cevm = new() { Message = string.Concat(ex.Message, "\n", $"Пользователь {author.Email} не смог добавить статью") };
                    return View("/Views/Alert/SomethingWrong.cshtml");
                }

                var model = _mapper.Map<AuthorViewModel>(author);
                model.ArticlesCount = _articleService.GetArticleDTOsByAuthor(author).Result.Length;
                model.CommentsCount = _commentService.GetCommentDTOsByAuthor(author).Result.Length;

                return View("/Views/Author/MyPage.cshtml", model);
            }

            _logger.LogWarning("Неверно заполнена форма");

            //// чтоб было возможно выбрать из всех тегов, а не только
            //// тех, что были переданны в этот метод
            var tagsName = _tagService.GetAllTagsDTOs().Result.Select(dto => dto.Content).ToList();

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
            AuthorDTO author = new();

            if (id == null)
            {
                author = await _authorService.GetAuthorDTOByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                _logger.LogInformation("Пользователь {email} перешёл к списку своих статей", author.Email);
            }
            if(id != null)
            {
                author = await _authorService.GetAuthorDTOById((int)id);

                _logger.LogInformation("Пользователь {currEmail} перешёл к списку статей автора {email}", HttpContext.User.Claims.FirstOrDefault().Value, author.Email);
            }

            var articles = await _articleService.GetArticleDTOsByAuthor(author);

            var avmArray = _mapper.Map<ArticleViewModel[]>(await _articleService.GetArticleDTOsByAuthor(author));

            for(int i = 0; i < articles.Length; i++)
            {
                avmArray[i].ArticleTags = _articleService.GetArticleTagsDTOs(articles[i]).Result.Select(dto => dto.Content).ToList();
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
            var articles = await _articleService.GetAllArticleDTOs();

            var avmArray = _mapper.Map<ArticleViewModel[]>(articles);

            for(int i = 0; i < articles.Length; i++)
            {
                avmArray[i].ArticleTags = _articleService.GetArticleTagsDTOs(articles[i]).Result.Select(dto => dto.Content).ToList();
            }

            _logger.LogInformation("Пользователь {email} перешёл к списку всех статей", HttpContext.User.Claims.FirstOrDefault().Value);

            return View("MyArticlesList", new ArticleListViewModel() { Articles = avmArray });
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/ReadArticle")]
        public async Task<IActionResult> ReadArticle(int id)
        {
            var article = await _articleService.GetArticleDTOById(id);

            var author = await _authorService.GetAuthorDTOById(article.AuthorId);

            var avm = _mapper.Map<ArticleViewModel>(article);

            var comments = await _commentService.GetCommentDTOsByArticle(article);

            foreach(var comment in comments.ToList())
            {
                var cvm = _mapper.Map<CommentViewModel>(comment);
                cvm.AuthorName = _authorService.GetAuthorDTOById(comment.AuthorId).Result.Email;
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
            // предотвращение доступа к изменению статьи без должной роли или без авторства
            try
            {
                var currAuthor = await _authorService.GetAuthorDTOByEmail(User.Claims.FirstOrDefault().Value);

                var article = await _articleService.GetArticleDTOById(id);

                if (!User.IsInRole("Moderator") && currAuthor.AuthorId != article.AuthorId)
                {
                    throw new CustomException($"Недостаточно прав для этого действия, {currAuthor.Email}");
                }

                UpdateArticleRequestViewModel uarvm = new() { ArticleId = article.ArticleId, NewTitle = article.Title, NewContent = article.Content };

                return View(uarvm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/AccessDenied.cshtml", cevm);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/RewriteArticle")]
        public async Task<IActionResult> ConfirmRewriteArticle([FromForm]UpdateArticleRequestViewModel model)
        {
            if(ModelState.IsValid)
            {
                var article = await _articleService.GetArticleDTOById(model.ArticleId);
                
                // для редиректа
                //var currentAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                var uar = _mapper.Map<UpdateArticleRequest>(model);

                await _articleService.UpdateArticle(article, uar);

                _logger.LogInformation("Пользователь {email} изменил {articleId} статью", HttpContext.User.Claims.FirstOrDefault().Value, article.ArticleId);

                return RedirectToAction("ArticleList");
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("RewriteArticle", model);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteArticle")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _articleService.DeleteArticle(id);

            _logger.LogInformation("Пользователем {email} была удалена статья {article}", HttpContext.User.Claims.FirstOrDefault().Value, id);

            return RedirectToAction("ArticleList");
        }
    }
}