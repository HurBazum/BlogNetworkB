using AutoMapper;
using BlogNetworkB.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Tag;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;
using BlogNetworkB.BLL.Services.Interfaces;
using System.Runtime.InteropServices;

namespace BlogNetworkB.Controllers
{
    [ExceptionHandler]
    [Route("/Tags")]
    public class TagController : Controller
    {
        readonly ITagService _tagService;
        readonly IMapper _mapper;
        readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, IMapper mapper, ILogger<TagController> logger)
        {
            _tagService = tagService;
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
                    var tag = (await _tagService.GetTagsDTOsByContent(tagViewModel.Content) != null) ? null : _mapper.Map<TagDTO>(tagViewModel);
                    if (tag != null)
                    {
                        await _tagService.AddTag(tag);

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
            var tags = await _tagService.GetAllTagsDTOs();

            var tagArray = _mapper.Map<TagViewModel[]>(tags);

            _logger.LogInformation("Пользователь {email} обратился ко списку всех тегов", HttpContext.User.Claims.FirstOrDefault().Value);

            return View(new TagListViewModel { Tags = tagArray });
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteTag")]
        public async Task<IActionResult> DeleteTag(int id)
        {

            try
            {
                await _tagService.DeleteTag(id);

                _logger.LogInformation("Пользователь {email} удалил тег {id}", HttpContext.User.Claims.FirstOrDefault().Value, id);
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
            try
            {
                var tag = await _tagService.GetTagDTOById(id) ?? throw new CustomException($"Непредвиденная ошибка");

                UpdateTagRequestViewModel utrvm = new() { TagId = tag.TagId, NewContent = tag.Content };

                return View(utrvm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/ChangeTag")]
        public async Task<IActionResult> ConfirmRewriteTag([FromForm]UpdateTagRequestViewModel utrvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(await _tagService.SearchSameTag(utrvm.NewContent))
                    {
                        throw new CustomException($"Тег с контентом \'{utrvm.NewContent}\' уже существует");
                    }

                    var tag = await _tagService.GetTagDTOById(utrvm.TagId);

                    var request = _mapper.Map<UpdateTagRequest>(utrvm);

                    await _tagService.UpdateTag(tag, request);

                    _logger.LogInformation("Пользователь {email} изменил тег {id}", HttpContext.User.Claims.FirstOrDefault().Value, tag.TagId);

                    return RedirectToAction("TagList");
                }

                int id = utrvm.TagId;

                _logger.LogWarning("Неверно заполнена форма");

                return RedirectToAction("RewriteTag", id);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                return View("/Views/Alert/SomethingWrong.cshtml", cevm);
            }
        }
    }
}