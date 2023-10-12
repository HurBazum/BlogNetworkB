using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.Models.Comment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;

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

            CommentListViewModel clvm = new() { Comments = _mapper.Map<CommentViewModel[]>(comments) };

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
                view.Author = author.FirstName + " " + author.LastName;
                view.Article = article.Title;
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

                int id = ccvm.ArticleId;

                return RedirectToAction("Index", "Home");
                //return RedirectToAction("ReadArticle", id);
            }

            return View("WriteComment", ccvm);
        }
    }
}
