using ConnectionLib.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BlogNetworkB.Models.Role;
using System.Security.Claims;

namespace BlogNetworkB.Controllers
{
    [Route("/Roles")]
    public class RoleController : Controller
    {
        readonly IRoleRepository _roleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IMapper _mapper;
        public RoleController(IRoleRepository roleRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
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
            if (!User.IsInRole("Admin"))
            {
                var myRoleList = HttpContext.User.Claims.Where(value => value.Type == ClaimsIdentity.DefaultRoleClaimType).Select(v => v.Value);

                MyRoleViewModel mrvm = new() { Roles = myRoleList.ToArray() };

                return View(mrvm);
            }
            else
            {
                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                var roles = await _authorRepository.GetAuthorsRoles(author);

                var rolesArray = _mapper.Map<RoleViewModel[]>(roles);

                return View("RoleList", new RoleListViewModel { Roles = rolesArray });
            }
        }

        #endregion

        #region Добавление роли пользователю

        [Authorize]
        [HttpGet]
        [Route("/[controller]/AuthorRoles/NewRole")]
        public IActionResult AddRole(int id) => View(new RoleViewModel { AuthorId = id });

        [Authorize]
        [HttpPost]
        [Route("/[controller]/AuthorRoles/NewRole")]
        public async Task<IActionResult> ConfirmAddRole([FromForm] RoleViewModel roleViewModel)
        {
            var role = await _roleRepository.GetRoleById(roleViewModel.RoleId);
            var author = await _authorRepository.GetAuthorById(roleViewModel.AuthorId);

            // т.к. метод get отказывается принимать
            // roleViewModel.AuthorId
            if (role == null)
            {
                int id = roleViewModel.AuthorId;
                return View("AddRole", id);
            }

            // если автор уже имеет такую роль - ничего не происходит
            bool hasRole = (_authorRepository.GetAuthorsRoles(author).Result.Contains(role) == true);
            if (hasRole)
            {
                return RedirectToAction("AuthorsList", "Author");
            }

            await _authorRepository.AddRole(author, role);

            return RedirectToAction("AuthorsList", "Author");
        }

        #endregion
    }
}
