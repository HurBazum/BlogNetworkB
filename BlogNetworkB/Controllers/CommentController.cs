using AutoMapper;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using BlogNetworkB.BLL.Models.Comment;
using ConnectionLib.DAL.Queries.Comment;

namespace BlogNetworkB.Controllers
{
    [Route("/Comments")]
    public class CommentController : Controller
    {
        readonly ICommentRepository _commentRepository;
        readonly IArticleRepository _articleRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IMapper _mapper;

        public CommentController(ICommentRepository commentRepository, IArticleRepository articleRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
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
            }
            if(id != null)
            {
                author = await _authorRepository.GetAuthorById((int)id);
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


                return RedirectToAction("Index", "Home");
            }

            return View("WriteComment", ccvm);
        }

        [Authorize]
        [Route("/[controller]/Author/EditComment")]
        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);
            UpdateCommentRequestViewModel ucrvm = new() { CommentId = comment.Id, NewContent = comment.Content };

            return View(ucrvm);
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

                return RedirectToAction("CommentsList");
            }
            return View("EditComment", ucvm.CommentId);
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/DeleteComment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            await _commentRepository.DeleteComment(comment);

            return RedirectToAction("CommentList");
        }
    }
}