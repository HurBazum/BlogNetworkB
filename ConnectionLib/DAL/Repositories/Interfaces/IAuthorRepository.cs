using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Author;

namespace ConnectionLib.DAL.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        public Task AddAuthor(Author author);
        public Task DeleteAuthor(Author author);
        public Task<Author> GetAuthorById(int id);
        public Task<Author> GetAuthorByEmail(string email);
        public Task<Author[]> GetAll();
        public Task UpdateAuthor(Author author, UpdateAuthorQuery updateAuthorQuery);
        public Task AddRole(Author author, Role role);
        public Task<Role[]> GetAuthorsRoles(Author author);
    }
}