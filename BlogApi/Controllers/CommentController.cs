using AutoMapper;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Models.Comment;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("/Api")]
    [ApiController]
    public class CommentController : Controller
    {
        readonly IMapper _mapper;
        readonly ICommentService _commentService;
        readonly IAuthorService _authorService;
        readonly IArticleService _articleService;

        public CommentController(
            IMapper mapper,
            ICommentService commentService,
            IArticleService articleService,
            IAuthorService authorService)
        {
            _mapper = mapper;
            _commentService = commentService;
            _articleService = articleService;
            _authorService = authorService;
        }

        [HttpGet]
        [Route("[controller]/All")]
        public async Task<IActionResult> CommentList()
        {
            var comments = await _commentService.GetAllCommentDTOs();

            return Ok(comments);
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public IActionResult GetOneComment([FromRoute]int id) 
        {
            var dto = _commentService.GetCommentDTOById(id);

            if(dto.IsCompletedSuccessfully)
            {
                return Ok(dto.Result);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("[controller]/CreateComment")]
        public async Task<IActionResult> CreateComment([FromBody]CreateCommentO createCommentO)
        {
            var dto = _mapper.Map<CommentDTO>(createCommentO);

            if(await _authorService.GetAuthorDTOById(dto.AuthorId) == null) return BadRequest();
            if(await _articleService.GetArticleDTOById(dto.ArticleId) == null) return BadRequest();

            await _commentService.AddComment(dto);

            return Ok();
        }

        [HttpDelete]
        [Route("[controller]/DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute]int id)
        {
            var dto = await _commentService.GetCommentDTOById(id);
            
            if(dto != null)
            {
                await _commentService.DeleteComment(dto);

                return Ok($"Комментарий №{id} успешно удалён");
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("[controller]/UpdateComment/{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute]int id, [FromBody]UpdateCommentRequest ucr)
        {
            var dto = await _commentService.GetCommentDTOById(id);

            if (dto != null)
            {
                await _commentService.UpdateComment(dto, ucr);
                
                return Ok();
            }

            return BadRequest();
        }
    }
}