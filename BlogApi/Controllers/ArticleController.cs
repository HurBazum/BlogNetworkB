using AutoMapper;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("/Api")]
    [ApiController]
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

        [HttpGet]
        [Route("[controller]/All")]
        public async Task<IActionResult> ArticleList()
        {
            var articles = await _articleService.GetAllArticleDTOs();

            foreach(var article in articles)
            {
                article.ArticleTagDTOs = _articleService.GetArticleTagsDTOs(article).Result.ToList();
            }

            return Ok(articles);
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public async Task<IActionResult> GetOneArticle([FromRoute]int id)
        {
            var dto = await _articleService.GetArticleDTOById(id);

            if (dto == null) return BadRequest();

            foreach (var tag in await _articleService.GetArticleTagsDTOs(dto))
            {
                dto.ArticleTagDTOs.Add(tag);
            }

            return Ok(dto);
        }

        [HttpPost]
        [Route("[controller]/CreateArticle")]
        public async Task<IActionResult> CreateArticle([FromBody]CreateArticleO createArticleO)
        {
            var dto = _mapper.Map<ArticleDTO>(createArticleO);

            if(await _articleService.GetArticleDTOById(dto.AuthorId) == null) return BadRequest();

            foreach(var tag in dto.ArticleTagDTOs)
            {
                if(!_tagService.GetTagDTOById(tag.TagId).Equals(tag))
                {
                    return BadRequest($"Ошибка в теге с id = {tag.TagId}");
                }
            }

            await _articleService.AddArticle(dto);

            return Ok();
        }

        [HttpDelete]
        [Route("[controller]/DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute]int id)
        {
            if(_articleService.GetArticleDTOById(id).Result == null) return BadRequest();

            await _articleService.DeleteArticle(id);

            return Ok();
        }

        [HttpPut]
        [Route("[controller]/UpdateArticle/{id}")]
        public async Task<IActionResult> UpdateArticle([FromRoute]int id, [FromBody]UpdateArticleRequest uar)
        {
            var dto = await _articleService.GetArticleDTOById(id);

            if(dto == null) return BadRequest();

            await _articleService.UpdateArticle(dto, uar);

            return Ok();
        }
    }
}