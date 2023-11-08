using BlogNetworkB.BLL.Models.Role;

namespace BlogNetworkB.BLL.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<RoleDTO[]> RoleDTOlist();
        public Task<RoleDTO> GetRoleDTOById(int id);
        public Task AddRole(RoleDTO roleDTO);
        public Task UpdateRole(RoleDTO roleDTO, UpdateRoleDescriptionRequest urdr);
        public Task DeleteRole(RoleDTO roleDTO);
    }
}