using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.Models.Account;
using Microsoft.AspNetCore.Mvc;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace BlogNetworkB.Controllers
{
    [Route("/Author")]
    public class AuthorController : Controller
    {
        readonly IAuthorRepository _authorRepository;
        readonly IRoleRepository _roleRepository;
        readonly IArticleRepository _articleRepository;
        readonly ICommentRepository _commentRepository;
        readonly IMapper _mapper;

        public AuthorController(IAuthorRepository authorRepository, IRoleRepository roleRepository, IArticleRepository articleRepository, ICommentRepository commentRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _roleRepository = roleRepository;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        [Route("/Register")]
        [HttpGet]
        public IActionResult Register() => View();

        /// <summary>
        /// Добавление пользователя со стандартной ролью "User".
        /// Роли надо добавлять в бд с помощью sql
        /// *Не работаю атрибуты
        /// </summary>
        [Route("/Register")]
        [HttpPost]
        public async Task<IActionResult> ConfirmRegister([FromForm] RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                // емейл должен быть уникальным
                bool authorExists = (_authorRepository.GetAuthorByEmail(registerViewModel.Email) == null);

                if (!authorExists)
                {
                    var author = _mapper.Map<Author>(registerViewModel);

                    author.Roles.Add(_roleRepository.GetAll().Result[0]);
                    author.Roles.Add(_roleRepository.GetRoleById(3).Result);

                    await _authorRepository.AddAuthor(author);

                    return RedirectToAction("Index", "Home");
                }

                return View("Register", registerViewModel);
            }
            return View("Register", registerViewModel);
        }

        [Route("/Login")]
        [HttpGet]
        public IActionResult Login() => View();

        [Route("/Login")]
        [HttpPost]
        public async Task<IActionResult> ConfirmLogin([FromForm] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid) 
            {
                var author = await _authorRepository.GetAuthorByEmail(loginViewModel.Email);


                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, author.Email)
                };

                var roles = await _authorRepository.GetAuthorsRoles(author);

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                var avm = _mapper.Map<AuthorViewModel>(author);

                avm.ArticlesCount = _articleRepository.GetArticlesByAuthor(author).Result.Length;
                avm.CommentsCount = _commentRepository.GetCommentByAuthor(author).Result.Length;

                return View("MyPage", avm);
            }

            return View("Login", loginViewModel);
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/MyBlog")]
        public IActionResult MyPage(AuthorViewModel model) => View();

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/All")]
        public async Task<IActionResult> AuthorsList()
        {
            var authors = await _authorRepository.GetAll();

            var authorsArray = _mapper.Map<AuthorViewModel[]>(authors);

            for(int i = 0; i < authors.Length; i++)
            {
                authorsArray[i].ArticlesCount = _articleRepository.GetArticlesByAuthor(authors[i]).Result.Length;
                authorsArray[i].CommentsCount = _commentRepository.GetCommentByAuthor(authors[i]).Result.Length;
                authorsArray[i].Roles = _authorRepository.GetAuthorsRoles(authors[i]).Result.Select(r => r.Name).ToList();
            }



            return View(new AuthorListViewModel { Authors = authorsArray });
        }
    }
}