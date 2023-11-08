using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    public class ArticleController : Controller
    {
        readonly IMapper _mapper;
        readonly IArticleService _articleService;
        readonly IAuthorService _authorService;
        readonly ICommentService _commentService;
        readonly ITagService _tagService;
        public ArticleController(
            IMapper mapper,
            IAuthorService authorService,
            IArticleService articleService,
            ICommentService commentService,
            ITagService tagService)
        {
            _mapper = mapper;
            _authorService = authorService;
            _articleService = articleService;
            _commentService = commentService;
            _tagService = tagService;
        }
    }
}