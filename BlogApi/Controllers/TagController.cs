using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("/Tag")]
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

        [HttpGet]
        [Route("/All")]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _tagService.GetAllTagsDTOs();

            return Ok(tags);
        }
    }
}