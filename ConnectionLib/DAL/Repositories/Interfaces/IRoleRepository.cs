using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Role;

namespace ConnectionLib.DAL.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role[]> GetAll();
        public Task<Role> GetRoleById(int id);
        public Task UpdateRole(Role role, UpdateRoleQuery urq);
        public Task DeleteRole(Role role);
        public Task AddRole(Role role);
    }
}