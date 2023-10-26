using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Comment;
using ConnectionLib.DAL.Queries.Comment;
using BlogNetworkB.Infrastructure.Exceptions;
using BlogNetworkB.Models.CustomError;

namespace BlogNetworkB.Controllers
{
    [Route("/Comments")]
    public class CommentController : Controller
    {
        readonly ICommentRepository _commentRepository;
        readonly IArticleRepository _articleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IMapper _mapper;
        readonly ILogger<CommentController> _logger;

        public CommentController(ICommentRepository commentRepository, IArticleRepository articleRepository, IAuthorRepository authorRepository, IMapper mapper, ILogger<CommentController> logger)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [Route("/[controller]/Author/Comments")]
        [HttpGet]
        public async Task<IActionResult> AuthorCommentsList(int? id)
        {
            Author author = new();

            if(id == null)
            {
                author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                _logger.LogInformation("Пользователь {email} обратился к своим комментариям", author.Email);
            }
            if(id != null)
            {
                author = await _authorRepository.GetAuthorById((int)id);

                _logger.LogInformation("Пользователь {currAuthor} обратился к комментариям пользователя {email}", HttpContext.User.Claims.FirstOrDefault().Value, author.Email);
            }

            var comments = await _commentRepository.GetCommentByAuthor(author);
            
            var cvm = _mapper.Map<CommentViewModel[]>(comments);


            for(int i = 0; i < comments.Length; i++)
            {
                cvm[i].AuthorName = author.Email;
                cvm[i].ArticleName = _articleRepository.GetArticleById(comments[i].ArticleId).Result.Title;
            }

            CommentListViewModel clvm = new() { Comments = cvm };

            return View("CommentsList", clvm);
        }

        [Authorize]
        [Route("/[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> CommentsList()
        {
            var comments = await _commentRepository.GetAll();

            var cvm = _mapper.Map<CommentViewModel[]>(comments);

            foreach (var view in cvm)
            {
                var author = _authorRepository.GetAuthorById(view.AuthorId).Result;
                var article = _articleRepository.GetArticleById(view.ArticleId).Result;
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
                var comment = _mapper.Map<Comment>(ccvm);

                var author = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);
                var article = await _articleRepository.GetArticleById(ccvm.ArticleId);

                comment.AuthorId = author.Id;
                comment.ArticleId = article.Id;

                await _commentRepository.AddComment(comment);

                _logger.LogInformation("Пользователь {email} оставил комментарий к статье {article}", author.Email, article.Id);

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

                var currAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                var comment = await _commentRepository.GetCommentById(isInt) ?? throw new CustomException($"Комментарий с id={id} не существует");

                UpdateCommentRequestViewModel ucrvm = new() { CommentId = comment.Id, NewContent = comment.Content };
                
                if (!User.IsInRole("Moderator") && currAuthor.Id != comment.AuthorId)
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
                var comment = await _commentRepository.GetCommentById(ucvm.CommentId);

                var ucr = _mapper.Map<UpdateCommentRequest>(ucvm);

                await _commentRepository.UpdateComment(comment, _mapper.Map<UpdateCommentQuery>(ucr));

                _logger.LogInformation("Пользователь {email} изменил коментарий {comment}", HttpContext.User.Claims.FirstOrDefault().Value, comment.Id);

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
                var currAuthor = await _authorRepository.GetAuthorByEmail(HttpContext.User.Claims.FirstOrDefault().Value);

                if (int.TryParse(id.ToString(), out int isInt) == false) throw new CustomException($"Некорректно задан id: {id}");


                var comment = await _commentRepository.GetCommentById(isInt) ?? throw new CustomException($"Комментария с id={id} не существует");


                if (!User.IsInRole("Moderator") || comment.AuthorId != currAuthor.Id) throw new CustomException($"Недостаточно прав, {currAuthor.Email}");

                await _commentRepository.DeleteComment(comment);

                _logger.LogInformation("Пользователь {email} удалил комментарий {comment}", HttpContext.User.Claims.FirstOrDefault().Value, comment.Id);

                return RedirectToAction("CommentList");
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