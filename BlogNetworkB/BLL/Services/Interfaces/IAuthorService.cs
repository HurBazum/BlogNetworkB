using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Role;

namespace BlogNetworkB.BLL.Services.Interfaces
{
    public interface IAuthorService
    {
        public Task<AuthorDTO[]> AuthorDTOlist();
        public Task AddAuthor(AuthorDTO authorDTO);
        public Task<AuthorDTO> GetAuthorDTOByEmail(string email);
        public Task<bool> AuthorExists(string email);
        public Task<RoleDTO[]> GetAuthorRoleDTOs(AuthorDTO authorDTO);
        public Task<AuthorDTO> GetAuthorDTOById(int id);
        public Task DeleteAuthor(AuthorDTO authorDTO);
        public Task UpdateAuthor(AuthorDTO authorDTO, UpdateAuthorRequest uar);
        public Task<AuthorDTO> GetCurrentAuthorDTO(HttpContext httpContext);
        public Task AddRoleToAuthor(AuthorDTO authorDTO, RoleDTO roleDTO);
        public Task DeleteAuthorRole(AuthorDTO authorDTO, RoleDTO roleDTO);
    }
}