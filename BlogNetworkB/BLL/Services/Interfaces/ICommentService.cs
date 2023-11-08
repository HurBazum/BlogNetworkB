using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Comment;

namespace BlogNetworkB.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        public Task AddComment(CommentDTO commentDTO);
        public Task<CommentDTO[]> GetCommentDTOsByAuthor(AuthorDTO authorDTO);
        public Task<CommentDTO[]> GetCommentDTOsByArticle(ArticleDTO articleDTO);
        public Task<CommentDTO[]> GetAllCommentDTOs();
        public Task<CommentDTO> GetCommentDTOById(int id);
        public Task UpdateComment(CommentDTO commentDTO, UpdateCommentRequest ucr);
        public Task DeleteComment(CommentDTO commentDTO);
    }
}