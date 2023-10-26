using ConnectionLib.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BlogNetworkB.Models.Role;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries;
using System.Security.Claims;
using BlogNetworkB.BLL.Models.Role;
using ConnectionLib.DAL.Queries.Role;
using Microsoft.AspNetCore.Mvc.Filters;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Roles")]
    public class RoleController : Controller
    {
        readonly IRoleRepository _roleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IMapper _mapper;
        readonly ILogger<RoleController> _logger;
        public RoleController(IRoleRepository roleRepository, IAuthorRepository authorRepository, IMapper mapper, ILogger<RoleController> logger)
        {
            _roleRepository = roleRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region получение списка ролей

        /// <summary>
        /// полный список ролей
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/[controller]/All")]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleRepository.GetAll();

            var rolesArray = _mapper.Map<RoleViewModel[]>(roles);

            return View(new RoleListViewModel { Roles = rolesArray });
        }

        /// <summary>
        /// роли конкретного пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/[controller]/MyRoles")]
        public async Task<IActionResult> MyRoleList() 
        {
            var myRoleList = HttpContext.User.Claims.Where(value => value.Type == ClaimsIdentity.DefaultRoleClaimType).Select(v => v.Value);

            MyRoleViewModel mrvm = new() { Roles = myRoleList.ToArray() };

            _logger.LogInformation("Пользователь {email} обратился к списку своих ролей", HttpContext.User.Claims.FirstOrDefault().Value);

            return View(mrvm);
        }

        #endregion

        #region Добавление/удаление роли пользователю

        [Authorize]
        [HttpGet]
        [Route("/[controller]/AuthorRoles/NewRole")]
        public IActionResult AddRole(int id) => View(new RoleViewModel { AuthorId = id });

        [Authorize]
        [HttpPost]
        [Route("/[controller]/AuthorRoles/NewRole")]
        public async Task<IActionResult> ConfirmAddRole([FromForm] RoleViewModel roleViewModel)
        {
            var admin = HttpContext.User.Claims.FirstOrDefault().Value;
            var role = await _roleRepository.GetRoleById(roleViewModel.RoleId);

            try
            {
                // т.к. метод get отказывается принимать
                // roleViewModel.AuthorId
                if (role == null)
                {
                    //ModelState.AddModelError("RoleId", "роль с таким id отсутствует");

                    throw new CustomException($"Пользователь {admin} пытался добавить роль с id={roleViewModel.RoleId}, такой роли нет");

                    //return View("AddRole", roleViewModel);
                }

                var author = await _authorRepository.GetAuthorById(roleViewModel.AuthorId);

                // если автор уже имеет такую роль - ничего не происходит
                bool hasRole = (_authorRepository.GetAuthorsRoles(author).Result.Contains(role) == true);
                if (hasRole)
                {
                    //ModelState.AddModelError("RoleId", "Пользователь уже имеет такую роль");

                    throw new CustomException($"Пользователь {admin} пытался добавить пользователю {author.Email} роль с id={roleViewModel.RoleId} повторно");
                }

                await _authorRepository.AddRole(author, role);

                _logger.LogInformation("Пользователь {admin} добавил пользователю {author} роль {id}", admin, author.Email, roleViewModel.RoleId);

                return RedirectToAction("AuthorsList", "Author");
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
        [Route("/[controller]/AuthorRoles/RemoveRole")]
        public IActionResult RemoveRole(int id) => View(new RoleViewModel { AuthorId = id});

        [Authorize]
        [HttpPost]
        [Route("/[controller]/AuthorRoles/RemoveRole")]
        public async Task<IActionResult> ConfirmRemoveRole([FromForm]RoleViewModel rvm)
        {
            var role = await _roleRepository.GetRoleById(rvm.RoleId);
            var author = await _authorRepository.GetAuthorById(rvm.AuthorId);

            try
            {
                // т.к. метод get отказывается принимать
                // roleViewModel.AuthorId
                if (role == null)
                {
                    throw new CustomException($"Роли с id={rvm.RoleId} не существует");
                }

                var authorRoles = await _authorRepository.GetAuthorsRoles(author);

                // если автор не имеет такую роль
                bool hasRole = authorRoles.Contains(role) == true;

                if (!hasRole)
                {
                    throw new CustomException($"Попытка удаления отсутвующей роли у пользователя");
                }

                if (hasRole && role.Name == "User")
                {
                    throw new CustomException("Попытка удаления базовой роли");
                }

                await _authorRepository.DeleteAuthorRole(author, role);

                _logger.LogInformation("Пользователь {admin} удалил у пользователя {email} роль {id}", HttpContext.User.Claims.FirstOrDefault().Value, author.Email, role.Id);

                return RedirectToAction("AuthorsList", "Author");
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new CustomErrorViewModel { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }

        #endregion

        [Authorize]
        [HttpGet]
        [Route("/[controller]/AddNewRole")]
        public IActionResult AddRoleToRoles() => View();

        [Authorize]
        [HttpPost]
        [Route("/[controller]/AddNewRole")]
        public async Task<IActionResult> ConfirmAddRoleToRoles([FromForm]RoleViewModel rvm)
        {
            if(ModelState.IsValid)
            {
                var role = _mapper.Map<Role>(rvm);

                try
                {
                    var sameRole = _roleRepository.GetAll().Result.Any(r => r.Name == role.Name);

                    if (sameRole != default)
                    {
                        throw new CustomException($"Попытка добавления дубликата. Роль с именем \'{role.Name}\' уже существует");
                    }

                    await _roleRepository.AddRole(role);

                    _logger.LogInformation("Пользователь {email} добавил роль {roleName}", HttpContext.User.Claims.FirstOrDefault().Value, role.Name);

                    return RedirectToAction("RoleList");
                }
                catch(Exception ex)
                {
                    _logger.LogError("{error}", ex.Message);
                    CustomErrorViewModel cevm = new() { Message = ex.Message };
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("AddRoleToRoles");
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/RewriteRole")]
        public async Task<IActionResult> RewriteRole(int id)
        {
            var role = await _roleRepository.GetRoleById(id);

            return View("EditRoleDescription", new UpdateRoleRequestViewModel { RoleId = id, RoleName = role.Name, NewDescription = role.Description });
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/RewriteRole")]
        public async Task<IActionResult> ConfirmRewriteRole([FromForm]UpdateRoleRequestViewModel urrvm)
        {
            if(ModelState.IsValid)
            {
                var request = _mapper.Map<UpdateRoleDescriptionRequest>(urrvm);

                var role = await _roleRepository.GetRoleById(urrvm.RoleId);

                await _roleRepository.UpdateRole(role, _mapper.Map<UpdateRoleQuery>(request));

                _logger.LogInformation("Пользователь {email} изменил описание у роли {id}", HttpContext.User.Claims.FirstOrDefault().Value, role.Id);

                return RedirectToAction("RoleList");
            }

            int id = urrvm.RoleId;

            _logger.LogWarning("Неверно заполнена форма");

            return RedirectToAction("EditRoleDescription", id);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteRole")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleRepository.GetRoleById(id);
            try
            {
                if (role.Name == "User" || role.Name == "Moderator" || role.Name == "Admin")
                {
                    throw new CustomException($"Попытка удалить одну из стандартных ролей: {role.Name}");
                }

                await _roleRepository.DeleteRole(role);

                _logger.LogInformation("Пользователь {email} удалил роль с {id}", HttpContext.User.Claims.FirstOrDefault().Value, role.Id);

                return RedirectToAction("RoleList");
            }
            catch(Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }
    }
}