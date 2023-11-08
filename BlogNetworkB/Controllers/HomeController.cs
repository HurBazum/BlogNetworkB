using AutoMapper;
using BlogNetworkB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogNetworkB.Models.Account;
using BlogNetworkB.BLL.Services.Interfaces;

namespace BlogNetworkB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IAuthorService _authorService;
        readonly ICommentService _commentService;
        readonly IArticleService _articleService;
        readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IMapper mapper,
            IAuthorService authorService,
            ICommentService commentService,
            IArticleService articleService)
        {
            _logger = logger;
            _mapper = mapper;
            _authorService = authorService;
            _commentService = commentService;
            _articleService = articleService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var author = await _authorService.GetCurrentAuthorDTO(HttpContext);
                var avm = _mapper.Map<AuthorViewModel>(author);

                avm.ArticlesCount = _articleService.GetArticleDTOsByAuthor(author).Result.Length;
                avm.CommentsCount = _commentService.GetCommentDTOsByAuthor(author).Result.Length;

                return View("/Views/Author/MyPage.cshtml", avm);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}