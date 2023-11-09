using AutoMapper;
using BlogNetworkB.BLL.Models.Tag;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("/Api")]
    [ApiController]
    public class TagController : Controller
    {
        readonly ITagService _tagService;
        readonly IMapper _mapper;

        public TagController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        // GET 
        [Route("[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> TagList()
        {
            var tags = await _tagService.GetAllTagsDTOs();

            return Ok(tags);
        }

        [Route("[controller]/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetOneTag([FromRoute] int id)
        {
            var dto = await _tagService.GetTagDTOById(id);

            if (dto != null)
            {
                return Ok(dto);
            }

            return BadRequest();
        }

        [Route("[controller]/Create")]
        [HttpPost]
        public IActionResult CreateTag([FromBody]CreateTagO tag)
        {
            var dto = _mapper.Map<TagDTO>(tag);

            if(_tagService.AddTag(dto).IsCompletedSuccessfully)
            {
                return Ok($"Тег успешно добавлен");
            }

            return BadRequest();
        }

        [Route("[controller]/Delete/{id}")]
        [HttpDelete]
        public IActionResult DeleteTag([FromRoute]int id)
        {
            if(_tagService.DeleteTag(id).IsCompletedSuccessfully)
            {
                return Ok($"Тег №{id} удалён успешно");
            }

            return BadRequest();
        }

        /// <summary>
        /// Что это
        /// </summary>
        /// <param name="id"></param>
        /// <param name="utr"></param>
        /// <returns></returns>
        [Route("[controller]/UpdateTag/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateTag([FromRoute]int id, [FromBody]UpdateTagRequest utr)
        {
            var dto = await _tagService.GetTagDTOById(id);

            if(dto == null)
            {
                return BadRequest();
            }

            if(_tagService.UpdateTag(dto, utr).IsCompletedSuccessfully)
            {
                return Ok($"Тег №{id} успешно изменён");
            }

            return BadRequest();
        }
    }
}