using AutoMapper;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Comment;
using BlogNetworkB.BLL.Services.Interfaces;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using BlogNetworkB.BLL.Models.Article;
using ConnectionLib.DAL.Queries.Comment;

namespace BlogNetworkB.BLL.Services
{
    public class CommentService : ICommentService
    {
        readonly IMapper _mapper;
        readonly ICommentRepository _commentRepository;
        readonly IAuthorRepository _authorRepository;
        readonly IArticleRepository _articleRepository;

        public CommentService(ICommentRepository commentRepository, IAuthorRepository authorRepository, IArticleRepository articleRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _authorRepository = authorRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        public async Task AddComment(CommentDTO commentDTO)
        {
            var comment = _mapper.Map<Comment>(commentDTO);

            await _commentRepository.AddComment(comment);
        }

        public async Task<CommentDTO[]> GetCommentDTOsByAuthor(AuthorDTO authorDTO)
        {
            var author = await _authorRepository.GetAuthorById(authorDTO.AuthorId);
            var comments = await _commentRepository.GetCommentByAuthor(author);

            return _mapper.Map<CommentDTO[]>(comments);
        }

        public async Task<CommentDTO[]> GetCommentDTOsByArticle(ArticleDTO articleDTO)
        {
            var article = await _articleRepository.GetArticleById(articleDTO.ArticleId);
            var comments = await _commentRepository.GetCommentByArticle(article);

            return _mapper.Map<CommentDTO[]>(comments);
        }

        public async Task<CommentDTO[]> GetAllCommentDTOs()
        {
            var comments = await _commentRepository.GetAll();

            return _mapper.Map<CommentDTO[]>(comments);
        }

        public async Task<CommentDTO> GetCommentDTOById(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            return _mapper.Map<CommentDTO>(comment);
        }

        public async Task UpdateComment(CommentDTO commentDTO, UpdateCommentRequest ucr)
        {
            var comment = await _commentRepository.GetCommentById(commentDTO.CommentId);

            await _commentRepository.UpdateComment(comment, _mapper.Map<UpdateCommentQuery>(ucr));
        }

        public async Task DeleteComment(CommentDTO commentDTO)
        {
            var comment = await _commentRepository.GetCommentById(commentDTO.CommentId);

            await _commentRepository.DeleteComment(comment);
        }
    }
}