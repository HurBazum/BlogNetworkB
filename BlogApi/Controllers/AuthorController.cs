using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    public class AuthorController : Controller
    {
        readonly IMapper _mapper;
        readonly IAuthorService _authorService;
        readonly IRoleService _roleService;
        readonly IArticleService _articleService;
        readonly ICommentService _commentService;

        public AuthorController(
            IMapper mapper,
            IAuthorService authorService,
            IRoleService roleService,
            IArticleService articleService,
            ICommentService commentService)
        {
            _mapper = mapper;
            _authorService = authorService;
            _roleService = roleService;
            _articleService = articleService;
            _commentService = commentService;
        }
    }
}