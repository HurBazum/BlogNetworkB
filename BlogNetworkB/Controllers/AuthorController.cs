using AutoMapper;
using BlogNetworkB.Models.Account;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.Infrastructure.Extensions;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Author")]
    public class AuthorController : Controller
    {
        readonly IMapper _mapper;
        readonly ILogger<AuthorController> _logger;
        readonly IAuthorService _authorService;
        readonly IRoleService _roleService;
        readonly IArticleService _articleService;
        readonly ICommentService _commentService;

        public AuthorController(
            IMapper mapper,
            ILogger<AuthorController> logger,
            IAuthorService authorService,
            IRoleService roleService,
            IArticleService articleService,
            ICommentService commentService)
        {
            _mapper = mapper;
            _logger = logger;
            _authorService = authorService;
            _roleService = roleService;
            _articleService = articleService;
            _commentService = commentService;
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
                if(_authorService.AuthorExists(registerViewModel.Email).Result)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже зарегистрирован");

                    _logger.LogError("Попытка зарегестрировать ещё одну страницу на {email}", registerViewModel.Email);

                    return View("Register");
                }

                registerViewModel.FirstName = registerViewModel.FirstName.UpFirstLowOther();
                registerViewModel.LastName = registerViewModel.LastName.UpFirstLowOther();

                var author = _mapper.Map<AuthorDTO>(registerViewModel);

                author.RoleDTOs.Add(_roleService.RoleDTOlist().Result[0]);

                if(_authorService.AuthorDTOlist().Result.Length == 0)
                {
                    author.RoleDTOs.Add(_roleService.RoleDTOlist().Result[3]);
                }

                try
                {
                    await _authorService.AddAuthor(author);
                }
                catch(Exception e)
                {
                    CustomErrorViewModel cevm = new() { Message = $"{author.RoleDTOs[0].RoleId}\n{author.RoleDTOs[0].Name}\n{author.RoleDTOs[0].Description}||{author.Email}=={author.AuthorId}=={author.RoleDTOs.Count}" };
                    _logger.LogError("{error}", e.Message);
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }

                _logger.LogInformation("Добавлен пользователь {email}: {roles}", registerViewModel.Email, author.RoleDTOs.RoleToString());

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
                var author = await _authorService.GetAuthorDTOByEmail(loginViewModel.Email);

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

                try
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, author.Email)
                    };

                    var roles = await _authorService.GetAuthorRoleDTOs(author);
                    _logger.LogWarning("role => {roles}", roles.RoleToString());
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{error}", ex.Message);
                    CustomErrorViewModel cevm = new() { Message = ex.Message };
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }
                
                var avm = _mapper.Map<AuthorViewModel>(author);

                avm.ArticlesCount = _articleService.GetArticleDTOsByAuthor(author).Result.Length;
                avm.CommentsCount = _commentService.GetCommentDTOsByAuthor(author).Result.Length;

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
                var author = _authorService.GetAuthorDTOById(isInt).Result ?? throw new CustomException($"Пользователя с id={id} не существует");
                
                var avm = _mapper.Map<AuthorViewModel>(author);

                avm.ArticlesCount = _articleService.GetArticleDTOsByAuthor(author).Result.Length;
                avm.CommentsCount = _commentService.GetCommentDTOsByAuthor(author).Result.Length;

                _logger.LogInformation("Пользователь {guest} перешёл на страницу пользователя {author}", HttpContext.User.Claims.FirstOrDefault().Value, avm.Email);

                return View("MyPage", avm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                if (ex.Message.Contains("не существует"))
                {
                    return View("/Views/Alert/NotFound.cshtml", cevm);
                }
                else
                {
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }
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
                var a = _authorService.GetAuthorDTOById((int)id).Result;

                var avm = _mapper.Map<AuthorViewModel>(a);

                avm.ArticlesCount = _articleService.GetArticleDTOsByAuthor(a).Result.Length;
                avm.CommentsCount = _commentService.GetCommentDTOsByAuthor(a).Result.Length;

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
            var authors = await _authorService.AuthorDTOlist();

            var authorsArray = _mapper.Map<AuthorViewModel[]>(authors);

            for(int i = 0; i < authors.Length; i++)
            {
                authorsArray[i].ArticlesCount = _articleService.GetArticleDTOsByAuthor(authors[i]).Result.Length;
                authorsArray[i].CommentsCount = _commentService.GetCommentDTOsByAuthor(authors[i]).Result.Length;
                authorsArray[i].Roles = _authorService.GetAuthorRoleDTOs(authors[i]).Result.Select(r => r.Name).ToList();
            }

            _logger.LogInformation("{email} перешёл к списку всех пользователей\n", HttpContext.User.Claims.FirstOrDefault().Value);

            return View(new AuthorListViewModel { Authors = authorsArray });
        }


        [Authorize]
        [HttpPost]
        [Route("/[controller]/Delete")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _authorService.GetAuthorDTOById(id);

                var currAuthor = HttpContext.User.Claims.FirstOrDefault().Value;

                if (author.Email == currAuthor)
                {
                    throw new CustomException($"Пользователь {author.Email} пытался удалить сам себя. Невозможное действие.");
                }
                else
                {
                    await _authorService.DeleteAuthor(author);

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
        public async Task<IActionResult> EditAuthor(int? id)
        {
            try
            {
                if(int.TryParse(id.ToString(), out int isInt) == false)
                {
                    throw new CustomException($"Некорректно задан id: {id}");
                }

                var currAuthor = await _authorService.GetAuthorDTOByEmail(User.Claims.FirstOrDefault().Value);

                if(!User.IsInRole("Moderator") && currAuthor.AuthorId != isInt)
                {
                    throw new CustomException($"Недостаточно прав, {currAuthor.Email}");
                }

                var author = await _authorService.GetAuthorDTOById(isInt) ?? throw new CustomException($"Пользователь с id={id} не существует");

                return View(new UpdateAuthorRequestViewModel { AuthorId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() {  Message = ex.Message };
                if(ex.Message.Contains("Недостаточно прав"))
                {
                    return View("/Views/Alert/AccessDenied.cshtml", cevm);
                }
                if(ex.Message.Contains("не существует"))
                {
                    return View("/Views/Alert/NotFound.cshtml", cevm);
                }
                else
                {
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }
            }
        }

        /// <summary>
        /// in progress. . .
        /// </summary>
        /// <param name="uarvm"></param>
        /// <returns></returns>
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
                var authAuthor = await _authorService.GetAuthorDTOByEmail(User.Claims.FirstOrDefault().Value);

                if (uarvm.AuthorId == null)
                {
                    var updateMeRequest = _mapper.Map<UpdateAuthorRequest>(uarvm);

                    await _authorService.UpdateAuthor(authAuthor, updateMeRequest);

                    _logger.LogInformation("Пользователь {id} сменил данные о себе", authAuthor.AuthorId);

                    if(updateMeRequest.NewEmail != null)
                    {
                        _logger.LogInformation("Пользователь {id} сменил email", authAuthor.AuthorId);

                        return RedirectToAction("Logout");
                    }

                    return RedirectToAction("Index", "Home");
                }

                var author = await _authorService.GetAuthorDTOById((int)uarvm.AuthorId);
                var uar = _mapper.Map<UpdateAuthorRequest>(uarvm);

                await _authorService.UpdateAuthor(author, uar);

                if (uar.NewEmail != null && authAuthor.AuthorId == uarvm.AuthorId)
                {
                    _logger.LogInformation("Пользователь {id} сменил email", authAuthor.AuthorId);

                    return RedirectToAction("Logout");
                }

                _logger.LogInformation("Пользователь {email} сменил данные пользователю {id}", authAuthor.Email, author.AuthorId);

                return RedirectToAction("AuthorsList");
            }

            int? authorId = uarvm.AuthorId;

            _logger.LogWarning("Неверно заполнены данные для редактирования анкеты");

            return View("EditAuthor", authorId);
        }

        ///<summary>
        /// 
        ///</summary>
        //async Task<AuthorListViewModel> GetAuthorsList()
        //{
        //    var authors = await _authorRepository.GetAll();

        //    var authorsArray = _mapper.Map<AuthorViewModel[]>(authors);

        //    for (int i = 0; i < authors.Length; i++)
        //    {
        //        authorsArray[i].ArticlesCount = _articleRepository.GetArticlesByAuthor(authors[i]).Result.Length;
        //        authorsArray[i].CommentsCount = _commentRepository.GetCommentByAuthor(authors[i]).Result.Length;
        //        authorsArray[i].Roles = _authorRepository.GetAuthorsRoles(authors[i]).Result.Select(r => r.Name).ToList();
        //    }

        //    return new AuthorListViewModel { Authors = authorsArray };
        //}
    }
}