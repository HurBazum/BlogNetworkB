using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Queries.Author;

namespace BlogNetworkB.DAL.Repositories.Interfaces
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