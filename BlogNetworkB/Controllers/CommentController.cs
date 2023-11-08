using AutoMapper;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Comment;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Models.Author;

namespace BlogNetworkB.Controllers
{
    [Route("/Comments")]
    public class CommentController : Controller
    {
        readonly IMapper _mapper;
        readonly ILogger<CommentController> _logger;
        readonly ICommentService _commentService;
        readonly IArticleService _articleService;
        readonly IAuthorService _authorService;

        public CommentController(
            IMapper mapper,
            ILogger<CommentController> logger,
            ICommentService commentService,
            IArticleService articleService,
            IAuthorService authorService)
        {
            _mapper = mapper;
            _logger = logger;
            _authorService = authorService;
            _commentService = commentService;
            _articleService = articleService;
        }

        [Authorize]
        [Route("/[controller]/Author/Comments")]
        [HttpGet]
        public async Task<IActionResult> AuthorCommentsList(int? id)
        {
            //Author author = new();
            AuthorDTO author = new();

            if(id == null)
            {
                author = await _authorService.GetAuthorDTOByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                _logger.LogInformation("Пользователь {email} обратился к своим комментариям", author.Email);
            }
            if(id != null)
            {
                author = await _authorService.GetAuthorDTOById((int)id);

                _logger.LogInformation("Пользователь {currAuthor} обратился к комментариям пользователя {email}", HttpContext.User.Claims.FirstOrDefault().Value, author.Email);
            }

            var comments = await _commentService.GetCommentDTOsByAuthor(author);
            
            var cvm = _mapper.Map<CommentViewModel[]>(comments);

            for(int i = 0; i < comments.Length; i++)
            {
                cvm[i].AuthorName = author.Email;
                cvm[i].ArticleName = _articleService.GetArticleDTOById(comments[i].ArticleId).Result.Title;
            }

            CommentListViewModel clvm = new() { Comments = cvm };

            return View("CommentsList", clvm);
        }

        [Authorize]
        [Route("/[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> CommentsList()
        {
            var comments = await _commentService.GetAllCommentDTOs();

            var cvm = _mapper.Map<CommentViewModel[]>(comments);

            foreach (var view in cvm)
            {
                var author = await _authorService.GetAuthorDTOById(view.AuthorId);
                var article = await _articleService.GetArticleDTOById(view.ArticleId);
                view.AuthorName = author.Email;
                view.ArticleName = article.Title;
            }

            _logger.LogInformation("Пользователь {email} обратился ко списку всех комментириев", HttpContext.User.Claims.FirstOrDefault().Value);

            return View(new CommentListViewModel { Comments = cvm });
        }

        [Authorize]
        [Route("/[controller]/WriteComment")]
        [HttpGet]
        public IActionResult WriteComment(int id) => View(new CreateCommentViewModel { ArticleId = id });

        [Authorize]
        [Route("/[controller]/WriteComment")]
        [HttpPost]
        public async Task<IActionResult> ConfirmWriteComment([FromForm] CreateCommentViewModel ccvm)
        {
            if (ModelState.IsValid)
            {
                var comment = _mapper.Map<CommentDTO>(ccvm);

                var author = await _authorService.GetCurrentAuthorDTO(HttpContext);//await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);
                var article = await _articleService.GetArticleDTOById(ccvm.ArticleId);//await _articleRepository.GetArticleById(ccvm.ArticleId);

                comment.AuthorId = author.AuthorId;
                comment.ArticleId = article.ArticleId;

                //await _commentRepository.AddComment(comment);
                await _commentService.AddComment(comment);

                _logger.LogInformation("Пользователь {email} оставил комментарий к статье {article}", author.Email, article.ArticleId);

                // ??
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("WriteComment", ccvm);
        }

        [Authorize]
        [Route("/[controller]/Author/EditComment")]
        [HttpGet]
        public async Task<IActionResult> EditComment(int? id)
        {
            try
            {
                if (int.TryParse(id.ToString(), out int isInt) == false)
                {
                    throw new CustomException($"Некорректно задан id: {id}");
                }

                var currAuthor = await _authorService.GetCurrentAuthorDTO(HttpContext);

                var comment = await _commentService.GetCommentDTOById(isInt) ?? throw new CustomException($"Комментарий с id={id} не существует");

                UpdateCommentRequestViewModel ucrvm = new() { CommentId = comment.CommentId, NewContent = comment.Content };
                
                if (!User.IsInRole("Moderator") && currAuthor.AuthorId != comment.AuthorId)
                {
                    throw new CustomException($"Недостаточно прав для этого действия, {currAuthor.Email}");
                }

                return View(ucrvm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{error}", ex.Message);
                CustomErrorViewModel cevm = new() { Message = ex.Message };
                if (ex.Message.Contains("Недостаточно прав"))
                {
                    return View("/Views/Alert/AccessDenied.cshtml", cevm);
                }
                if(ex.Message.Contains("не существует"))
                {
                    return View("/Views/Alert/NotFound.cshtml", cevm);
                }
                else
                {
                    return View("/Views/Alert/SomethingWrong.cshtml", cevm);
                }
            }
        }

        [Authorize]
        [Route("/[controller]/Author/EditComment")]
        [HttpPost]
        public async Task<IActionResult> ConfirmEditComment([FromForm] UpdateCommentRequestViewModel ucvm)
        {
            if (ModelState.IsValid)
            {
                var comment = await _commentService.GetCommentDTOById(ucvm.CommentId);

                var ucr = _mapper.Map<UpdateCommentRequest>(ucvm);

                await _commentService.UpdateComment(comment, ucr);

                _logger.LogInformation("Пользователь {email} изменил коментарий {comment}", HttpContext.User.Claims.FirstOrDefault().Value, comment.CommentId);

                return RedirectToAction("CommentsList");
            }

            _logger.LogWarning("Неверно заполнена форма");

            return View("EditComment", ucvm.CommentId);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteComment")]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            try
            {
                var currAuthor = await _authorService.GetCurrentAuthorDTO(HttpContext);

                if (int.TryParse(id.ToString(), out int isInt) == false) throw new CustomException($"Некорректно задан id: {id}");

                var comment = await _commentService.GetCommentDTOById(isInt) ?? throw new CustomException($"Комментария с id={id} не существует");

                if (!User.IsInRole("Moderator") && comment.AuthorId != currAuthor.AuthorId) throw new CustomException($"Недостаточно прав, {currAuthor.Email}");

                await _commentService.DeleteComment(comment);

                _logger.LogInformation("Пользователь {email} удалил комментарий {comment}", HttpContext.User.Claims.FirstOrDefault().Value, comment.CommentId);

                return RedirectToAction("CommentsList");
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