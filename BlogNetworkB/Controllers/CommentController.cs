using AutoMapper;
using BlogNetworkB.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace BlogNetworkB.Controllers
{
    [Route("/Comments")]
    public class CommentController : Controller
    {
        readonly ICommentRepository _commentRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IMapper _mapper;

        public CommentController(ICommentRepository commentRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [Route("/[controller]/All")]
        [HttpGet]
        public async Task<IActionResult> CommentsList()
        {
            return View();
        }
    }
}
