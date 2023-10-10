using BlogNetworkB.DAL.Enteties;

namespace BlogNetworkB.DAL.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role[]> GetAll();
        public Task<Role> GetRoleById(int id);
    }
}