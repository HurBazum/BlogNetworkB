using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Role;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("/Api")]
    [ApiController]
    public class AuthorController : Controller
    {
        readonly IMapper _mapper;
        readonly IAuthorService _authorService;
        readonly IRoleService _roleService;

        public AuthorController(
            IMapper mapper,
            IAuthorService authorService,
            IRoleService roleService)
        {
            _mapper = mapper;
            _authorService = authorService;
            _roleService = roleService;
        }
        
        [HttpGet]
        [Route("[controller]/All")]
        public async Task<IActionResult> AuthorList()
        {
            var authors = await _authorService.AuthorDTOlist();

            foreach (var author in authors)
            {
                author.RoleDTOs = _authorService.GetAuthorRoleDTOs(author).Result.ToList();
            }

            return Ok(authors);
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public async Task<IActionResult> GetOneAuthor([FromRoute]int id) 
        { 
            var dto = await _authorService.GetAuthorDTOById(id);

            if (dto == null) return BadRequest();

            dto.RoleDTOs = _authorService.GetAuthorRoleDTOs(dto).Result.ToList();

            return Ok(dto);
        }

        /// <summary>
        /// ??
        /// </summary>
        /// <param name="createAuthorO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/CreateAuthor")]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorO createAuthorO)
        {
            var dto = _mapper.Map<AuthorDTO>(createAuthorO);

            dto.RoleDTOs.Add(_roleService.GetRoleDTOById(1).Result);

            if (_authorService.AuthorExists(dto.Email).Result == true) return BadRequest($"{dto.Email} already exists");

            await _authorService.AddAuthor(dto);

            return Ok();
        }

        [HttpDelete]
        [Route("[controller]/DeleteAuthor/{id}")]
        public async Task<IActionResult> DeleteAuthor([FromRoute]int id)
        {
            var dto = await _authorService.GetAuthorDTOById(id);

            if(dto == null) return BadRequest();

            await _authorService.DeleteAuthor(dto);
            
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uar"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[controller]/UpdateAuthor/{id}")]
        public async Task<IActionResult> UpdateAuthor([FromRoute]int id, [FromBody]UpdateAuthorRequest uar)
        {
            var dto = await _authorService.GetAuthorDTOById(id);

            if (dto == null) return BadRequest();

            await _authorService.UpdateAuthor(dto, uar);

            return Ok();
        }
    }
}