using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BlogNetworkB.Controllers
{
    [Route("/Tags")]
    public class TagController : Controller
    {
        readonly ITagRepository _tagRepository;
        readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }


        [Authorize]
        [HttpGet]
        public IActionResult CreateTag() => View();

        [Authorize]
        [Route("/[controller]/Create_tag")]
        [HttpPost]
        public async Task<IActionResult> ConfirmCreateTag([FromForm] TagViewModel tagViewModel)
        {
            if(ModelState.IsValid)
            {
                // проверка на наличие тега с таким же контентом
                var tag = (await _tagRepository.GetTagByContent(tagViewModel.Content) != null) ? null : _mapper.Map<Tag>(tagViewModel);

                if(tag != null)
                {
                    await _tagRepository.AddTag(tag);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // ?
                    return View("Такой тег уже есть");
                }
            }

            return View("CreateTag", tagViewModel);
        }

        [Route("/[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> TagList()
        {
            var tags = await _tagRepository.GetAll();

            var tagArray = _mapper.Map<TagViewModel[]>(tags);

            return View(new TagListViewModel { Tags = tagArray });
        }
    }
}
