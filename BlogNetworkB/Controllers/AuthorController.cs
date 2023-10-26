using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using BlogNetworkB.Models.Account;
using Microsoft.AspNetCore.Mvc;
using ConnectionLib.DAL.Enteties;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.Infrastructure.Extensions;
using BlogNetworkB.BLL.Models.Author;
using ConnectionLib.DAL.Queries.Author;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Author")]
    public class AuthorController : Controller
    {
        readonly IAuthorRepository _authorRepository;
        readonly IRoleRepository _roleRepository;
        readonly IArticleRepository _articleRepository;
        readonly ICommentRepository _commentRepository;
        readonly IMapper _mapper;
        readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorRepository authorRepository, IRoleRepository roleRepository, IArticleRepository articleRepository, ICommentRepository commentRepository, IMapper mapper, ILogger<AuthorController> logger)
        {
            _authorRepository = authorRepository;
            _roleRepository = roleRepository;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [Route("/[controller]/Register")]
        [HttpGet]
        public IActionResult Register() => View();

        /// <summary>
        /// Добавление пользователя со стандартной ролью "User".
        /// Роли надо добавлять в бд с помощью sql
        /// *Не работаю атрибуты
        /// </summary>
        [Route("/[controller]/Register")]
        [HttpPost]
        public async Task<IActionResult> ConfirmRegister([FromForm] RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                // емейл должен быть уникальным
                var authorExists = await _authorRepository.GetAuthorByEmail(registerViewModel.Email);

                if(authorExists != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже зарегистрирован");

                    _logger.LogError("Попытка зарегестрировать ещё одну страницу на {email}", registerViewModel.Email);

                    return View("Register");
                }

                registerViewModel.FirstName = registerViewModel.FirstName.UpFirstLowOther();
                registerViewModel.LastName = registerViewModel.LastName.UpFirstLowOther();

                var author = _mapper.Map<Author>(registerViewModel);

                author.Roles.Add(_roleRepository.GetAll().Result[0]);

                int countAuthors = _authorRepository.GetAll().Result.Length;

                // создаём первого администратора
                // он может добавлять роли остальным пользователям
                if (countAuthors == 0)
                {                    
                    author.Roles.Add(_roleRepository.GetRoleById(3).Result);
                }

                await _authorRepository.AddAuthor(author);

                _logger.LogInformation("Добавлен пользователь {email}: {roles}", registerViewModel.Email, author.Roles.ConvertToString());

                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("Register", registerViewModel);
        }

        [Route("/[controller]/Login")]
        [HttpGet]
        public IActionResult Login() => View();

        [Route("/[controller]/Login")]
        [HttpPost]
        public async Task<IActionResult> ConfirmLogin([FromForm] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid) 
            {
                var author = await _authorRepository.GetAuthorByEmail(loginViewModel.Email);

                if(author == null)
                {
                    ModelState.AddModelError("Email", "неверный email");
                    _logger.LogWarning("Некорректный email");
                    return View("Login");
                }

                if(author!.Password != loginViewModel.Password)
                {
                    ModelState.AddModelError("Password", "неверный пароль");
                    _logger.LogWarning("Неверный пароль - {email}", loginViewModel.Email);
                    return View("Login");
                }

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

                _logger.LogInformation("Пользователь {email} авторизовался", author.Email);
                
                return View("MyPage", avm);
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("Login", loginViewModel);
        }


        [Authorize]
        [HttpGet]
        [Route("/[controller]/Blog")]
        public IActionResult AuthorPage(int? id)
        {
            try
            {
                if(id == null || int.TryParse(id.ToString(), out int isInt) == false)
                {
                    throw new CustomException($"Некорректно задан id: \'{id}\'");
                }

                // выражение объединения
                var a = _authorRepository.GetAuthorById(isInt).Result ?? throw new CustomException($"Пользователя с id={id} не существует");
                
                var avm = _mapper.Map<AuthorViewModel>(a);

                avm.ArticlesCount = _articleRepository.GetArticlesByAuthor(a).Result.Length;
                avm.CommentsCount = _commentRepository.GetCommentByAuthor(a).Result.Length;

                _logger.LogInformation("Пользователь {guest} перешёл на страницу пользователя {author}", HttpContext.User.Claims.FirstOrDefault().Value, avm.Email);

                return View("MyPage", avm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }

        // in progress. . .
        [Authorize]
        [HttpGet]
        [Route("/[controller]/MyBlog")]
        public IActionResult MyPage(AuthorViewModel? model, int? id)
        {
            if (model != null)
            {
                return View();
            }
            else
            {
                var a = _authorRepository.GetAuthorById((int)id).Result;

                var avm = _mapper.Map<AuthorViewModel>(a);

                avm.ArticlesCount = _articleRepository.GetArticlesByAuthor(a).Result.Length;
                avm.CommentsCount = _commentRepository.GetCommentByAuthor(a).Result.Length;

                return View(avm);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string email = HttpContext.User.Claims.FirstOrDefault().Value;

            await HttpContext.SignOutAsync("Cookies");

            _logger.LogInformation("Пользователь {email} вышел из сети", email);

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

            _logger.LogInformation("{email} перешёл к списку всех пользователей\n", HttpContext.User.Claims.FirstOrDefault().Value);

            return View(new AuthorListViewModel { Authors = authorsArray });
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.GetAuthorById(id);

                var currAuthor = HttpContext.User.Claims.FirstOrDefault().Value;

                if (author.Email == currAuthor)
                {
                    throw new CustomException($"Пользователь {author.Email} пытался удалить сам себя. Невозможное действие.");
                }
                else
                {
                    await _authorRepository.DeleteAuthor(author);

                    _logger.LogInformation("Пользователь {email} удалил пользователя {author}", currAuthor, author.Email);

                    return RedirectToAction("AuthorsList");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/Edit")]
        public IActionResult EditAuthor(int? id) => View(new UpdateAuthorRequestViewModel { AuthorId = id});

        
        [Authorize]
        [HttpPost]
        [Route("/[controller]/Edit")]
        public async Task<IActionResult> ConfirmEditAuthor([FromForm] UpdateAuthorRequestViewModel uarvm)
        {
            if(ModelState.IsValid)
            {
                if(uarvm.NewFirstName != null)
                {
                    uarvm.NewFirstName = CaseExtension.UpFirstLowOther(uarvm.NewFirstName);
                }

                if(uarvm.NewLastName != null)
                {
                    uarvm.NewLastName = CaseExtension.UpFirstLowOther(uarvm.NewLastName);
                }

                // при смене имейла текущего пользователя, будет выполняться Logout()
                var authAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                if (uarvm.AuthorId == null)
                {
                    var updateMeRequest = _mapper.Map<UpdateAuthorRequest>(uarvm);

                    await _authorRepository.UpdateAuthor(authAuthor, _mapper.Map<UpdateAuthorQuery>(updateMeRequest));

                    _logger.LogInformation("Пользователь {id} сменил данные о себе", authAuthor.Id);

                    if(updateMeRequest.NewEmail != null)
                    {
                        _logger.LogInformation("Пользователь {id} сменил email", authAuthor.Id);

                        return RedirectToAction("Logout");
                    }

                    return RedirectToAction("Index", "Home");
                }

                var author = await _authorRepository.GetAuthorById((int)uarvm.AuthorId);
                var uar = _mapper.Map<UpdateAuthorRequest>(uarvm);

                await _authorRepository.UpdateAuthor(author, _mapper.Map<UpdateAuthorQuery>(uar));

                if (uar.NewEmail != null && authAuthor.Id == uarvm.AuthorId)
                {
                    _logger.LogInformation("Пользователь {id} сменил email", authAuthor.Id);

                    return RedirectToAction("Logout");
                }

                _logger.LogInformation("Пользователь {email} сменил данные пользователю {id}", authAuthor.Email, author.Id);

                return RedirectToAction("AuthorsList");
            }

            int? authorId = uarvm.AuthorId;

            _logger.LogWarning("Неверно заполнены данные для редактирования анкеты");

            return View("EditAuthor", authorId);
        }

        async Task<AuthorListViewModel> GetAuthorsList()
        {
            var authors = await _authorRepository.GetAll();

            var authorsArray = _mapper.Map<AuthorViewModel[]>(authors);

            for (int i = 0; i < authors.Length; i++)
            {
                authorsArray[i].ArticlesCount = _articleRepository.GetArticlesByAuthor(authors[i]).Result.Length;
                authorsArray[i].CommentsCount = _commentRepository.GetCommentByAuthor(authors[i]).Result.Length;
                authorsArray[i].Roles = _authorRepository.GetAuthorsRoles(authors[i]).Result.Select(r => r.Name).ToList();
            }

            return new AuthorListViewModel { Authors = authorsArray };
        }
    }
}