using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    public class CommentController : Controller
    {
        readonly IMapper _mapper;
        readonly ICommentService _commentService;
        readonly IArticleService _articleService;
        readonly IAuthorService _authorService;

        public CommentController(
            IMapper mapper,
            ICommentService commentService,
            IArticleService articleService,
            IAuthorService authorService)
        {
            _mapper = mapper;
            _authorService = authorService;
            _commentService = commentService;
            _articleService = articleService;
        }
    }
}