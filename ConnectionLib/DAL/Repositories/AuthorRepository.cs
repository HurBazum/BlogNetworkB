using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Author;
using ConnectionLib.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConnectionLib.DAL.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        readonly BlogContext _blogContext;

        public AuthorRepository(BlogContext blogContext) => _blogContext = blogContext;

        public async Task AddAuthor(Author author)
        {
            var entry = _blogContext.Authors.Entry(author);
            if (entry.State == EntityState.Detached)
            {
                await _blogContext.AddAsync(author);
                await _blogContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// удаление автора
        /// </summary>
        public async Task DeleteAuthor(Author author)
        {
            var entry = _blogContext.Authors.Entry(author);
            entry.State = EntityState.Deleted;
            await _blogContext.SaveChangesAsync();
        }


        public async Task UpdateAuthor(Author author, UpdateAuthorQuery updateAuthorQuery)
        {
            author = AuthorConverter.Convert(author, updateAuthorQuery);
            var entry = _blogContext.Authors.Entry(author);
            if (entry.State == EntityState.Detached)
            {
                _blogContext.Authors.Update(author);
            }
            await _blogContext.SaveChangesAsync();
        }

        public async Task AddRole(Author author, Role role)
        {
            author.Roles.Add(role);
            var entry = _blogContext.Entry(author);
            entry.State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }

        public async Task DeleteAuthorRole(Author author, Role role)
        {
            var a = _blogContext.Authors.Include(a => a.Roles).Where(a => a.Id == author.Id).FirstOrDefault();
            a.Roles.Remove(role);
            await _blogContext.SaveChangesAsync();
        }

        public async Task<Author> GetAuthorById(int id) => await _blogContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
        public async Task<Author> GetAuthorByEmail(string email) => await _blogContext.Authors.FirstOrDefaultAsync(a => a.Email == email);
        public async Task<Author[]> GetAll() => await _blogContext.Authors.ToArrayAsync();

        /// <summary>
        /// получение ролей автора
        /// </summary>
        public async Task<Role[]> GetAuthorsRoles(Author author) => await _blogContext.Authors.Where(a => a.Id == author.Id).SelectMany(a => a.Roles).ToArrayAsync();
    }
}