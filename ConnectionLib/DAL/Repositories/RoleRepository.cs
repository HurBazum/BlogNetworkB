using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Role;
using ConnectionLib.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConnectionLib.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        readonly BlogContext _blogContext;
        public RoleRepository(BlogContext blogContext) => _blogContext = blogContext;

        public async Task AddRole(Role role)
        {
            var entry = _blogContext.Roles.Entry(role);
            if(entry.State == EntityState.Detached)
            {
                await _blogContext.Roles.AddAsync(role);
                await _blogContext.SaveChangesAsync();
            }
        }

        public async Task DeleteRole(Role role)
        {
            var entry = _blogContext.Roles.Entry(role);
            entry.State = EntityState.Deleted;
            await _blogContext.SaveChangesAsync();
        }

        public async Task UpdateRole(Role role, UpdateRoleQuery urq)
        {
            role = RoleConverter.Convert(role, urq);
            var entry = _blogContext.Roles.Entry(role);
            entry.State = EntityState.Modified;
            await _blogContext.SaveChangesAsync();
        }

        public async Task<Role[]> GetAll() => await _blogContext.Roles.ToArrayAsync();
        public async Task<Role> GetRoleById(int id) => await _blogContext.Roles.FirstOrDefaultAsync(x => x.Id == id);

    }
}