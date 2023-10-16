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

            // т.к. метод get отказывается принимать
            // roleViewModel.AuthorId
            if (role == null)
            {
                int id = rvm.AuthorId;
                return View("RemoveRole", id);
            }

            // если автор не имеет такую роль
            bool hasRole = (_authorRepository.GetAuthorsRoles(author).Result.Contains(role) != true);
            if (hasRole)
            {
                return View("/Views/Alert/SomethingWrong.cshtml");
            }

            await _authorRepository.DeleteAuthorRole(author, role);

            return RedirectToAction("AuthorsList", "Author");
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
                    await _roleRepository.AddRole(role);

                    return RedirectToAction("RoleList");
                }
                catch
                {
                    return View("/Views/Alert/SomethingWrong.cshtml");
                }
            }

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

                return RedirectToAction("RoleList");
            }

            int id = urrvm.RoleId;

            return RedirectToAction("EditRoleDescription", id);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteRole")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleRepository.GetRoleById(id);
            await _roleRepository.DeleteRole(role);
            return RedirectToAction("RoleList");
        }
    }
}
