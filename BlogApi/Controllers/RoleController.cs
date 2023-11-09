using AutoMapper;
using BlogNetworkB.BLL.Models.Role;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BlogApi.Controllers
{
    [Route("/Api")]
    [ApiController]
    public class RoleController : Controller
    {
        readonly IRoleService _roleService;
        readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [Route("[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleService.RoleDTOlist();

            return Ok(roles);
        }

        [Route("[controller]/{id}")]
        [HttpGet]
        public IActionResult GetOneRole([FromRoute]int id)
        {
            var role = _roleService.GetRoleDTOById(id);

            if (role.IsCompletedSuccessfully)
            {
                return Ok(role.Result);
            }

            return BadRequest($"роли с id={id} не существует");
        }

        [Route("[controller]/Create")]
        [HttpPost]
        public IActionResult CreateRole([FromBody]CreateRoleO createRoleO)
        {
            var dto = _mapper.Map<RoleDTO>(createRoleO);

            _roleService.AddRole(dto);

            return Ok("Роль успешно добавлена");
        }

        [Route("[controller]/Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromRoute]int id)
        {
            var dto = await _roleService.GetRoleDTOById(id);

            if(dto != null)
            {
                await _roleService.DeleteRole(dto);

                return Ok($"Роль №{id} успешно удалена");
            }

            return BadRequest();
        }

        [Route("[controller]/UpdateRole/{id}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateRole([FromRoute]int id, [FromBody]UpdateRoleDescriptionRequest urdr)
        {
            var dto = await _roleService.GetRoleDTOById(id);

            if(dto != null)
            {
                await _roleService.UpdateRole(dto, urdr);

                return Ok("Роль успешно изменена");
            }

            return BadRequest();
        }
    }
}