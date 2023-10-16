using ConnectionLib.DAL.Enteties;

namespace ConnectionLib.DAL.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role[]> GetAll();
        public Task<Role> GetRoleById(int id);
    }
}