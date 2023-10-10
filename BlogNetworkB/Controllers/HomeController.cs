using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogNetworkB.Models.Account;

namespace BlogNetworkB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IAuthorRepository _authorRepository;
        readonly ICommentRepository _commentRepository;
        readonly IArticleRepository _articleRepository;
        readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IAuthorRepository authorRepository, IArticleRepository articleRepository, ICommentRepository commentRepository, IMapper mapper)
        {
            _logger = logger;
            _authorRepository = authorRepository;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);
                var avm = _mapper.Map<AuthorViewModel>(author);

                avm.ArticlesCount = _articleRepository.GetArticlesByAuthor(author).Result.Length;
                avm.CommentsCount = _commentRepository.GetCommentByAuthor(author).Result.Length;

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