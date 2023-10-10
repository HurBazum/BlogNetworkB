using BlogNetworkB.DAL.Repositories.Interfaces;
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

        [Authorize]
        [HttpGet]
        [Route("/[controller]/All")]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleRepository.GetAll();

            var rolesArray = _mapper.Map<RoleViewModel[]>(roles);

            return View(new RoleListViewModel { Roles = rolesArray });
        }

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
    }
}
