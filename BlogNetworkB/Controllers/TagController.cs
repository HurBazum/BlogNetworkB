using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using BlogNetworkB.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Tag;
using ConnectionLib.DAL.Queries.Tag;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Tags")]
    public class TagController : Controller
    {
        readonly ITagRepository _tagRepository;
        readonly IMapper _mapper;
        readonly ILogger<TagController> _logger;

        public TagController(ITagRepository tagRepository, IMapper mapper, ILogger<TagController> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize]
        [HttpGet]
        public IActionResult CreateTag() => View();

        [Authorize]
        [Route("/[controller]/Create_tag")]
        [HttpPost]
        public async Task<IActionResult> ConfirmCreateTag([FromForm] TagViewModel tagViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // проверка на наличие тега с таким же контентом
                    var tag = (await _tagRepository.GetTagByContent(tagViewModel.Content) != null) ? null : _mapper.Map<Tag>(tagViewModel);

                    if (tag != null)
                    {
                        await _tagRepository.AddTag(tag);

                        _logger.LogInformation("Пользователь {email} добавил тег {tagContent}", HttpContext.User.Claims.FirstOrDefault().Value, tag.Content);

                        return RedirectToAction("TagList");
                    }
                    else
                    {
                        throw new CustomException($"Тег с контентом \'{tagViewModel.Content}\' уже существует");
                    }
                }

                return View("CreateTag", tagViewModel);
            }
            catch(Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }

        [HttpGet]
        [Route("/[controller]/All")]
        public async Task<IActionResult> TagList()
        {
            var tags = await _tagRepository.GetAll();

            var tagArray = _mapper.Map<TagViewModel[]>(tags);

            _logger.LogInformation("Пользователь {email} обратился ко списку всех тегов", HttpContext.User.Claims.FirstOrDefault().Value);

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
                _logger.LogInformation("Пользователь {email} удалил тег {id}", HttpContext.User.Claims.FirstOrDefault().Value, tag.Id);

            }
            catch(Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
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

                _logger.LogInformation("Пользователь {email} изменил тег {id}", HttpContext.User.Claims.FirstOrDefault().Value, tag.Id);

                return RedirectToAction("TagList");
            }

            int id = utrvm.TagId;

            _logger.LogWarning("Неверно заполнена форма");

            return RedirectToAction("RewriteTag", id);
        }
    }
}