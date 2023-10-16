using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using BlogNetworkB.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Tag;
using ConnectionLib.DAL.Queries.Tag;

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

        [HttpGet]
        [Route("/[controller]/All")]
        public async Task<IActionResult> TagList()
        {
            var tags = await _tagRepository.GetAll();

            var tagArray = _mapper.Map<TagViewModel[]>(tags);

            return View(new TagListViewModel { Tags = tagArray });
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteTag")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _tagRepository.GetTagById(id);

            try
            {
                await _tagRepository.DeleteTag(tag);
            }
            catch
            {
                return View("/Views/Alert/SomethingWrong.cshtml");
            }

            return RedirectToAction("TagList");
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/ChangeTag")]
        public async Task<IActionResult> RewriteTag(int id)
        {
            var tag = await _tagRepository.GetTagById(id);

            UpdateTagRequestViewModel utrvm = new() { TagId = tag.Id, NewContent = tag.Content };

            return View(utrvm);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/ChangeTag")]
        public async Task<IActionResult> ConfirmRewriteTag([FromForm]UpdateTagRequestViewModel utrvm)
        {
            if (ModelState.IsValid)
            {
                var tag = await _tagRepository.GetTagById(utrvm.TagId);

                var request = _mapper.Map<UpdateTagRequest>(utrvm);

                await _tagRepository.UpdateTag(tag, _mapper.Map<UpdateTagQuery>(request));

                return RedirectToAction("TagList");
            }

            int id = utrvm.TagId;
            return RedirectToAction("RewriteTag", id);
        }
    }
}
