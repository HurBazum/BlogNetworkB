using AutoMapper;
using BlogNetworkB.BLL.Models.Role;
using BlogNetworkB.BLL.Services.Interfaces;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Role;

namespace BlogNetworkB.BLL.Services
{
    public class RoleService : IRoleService
    {
        readonly IMapper _mapper;
        readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDTO[]> RoleDTOlist()
        {
            var roles = await _roleRepository.GetAll();

            return _mapper.Map<RoleDTO[]>(roles);
        }

        public async Task<RoleDTO> GetRoleDTOById(int id)
        {
            var role = await _roleRepository.GetRoleById(id);

            return _mapper.Map<RoleDTO>(role);
        }

        public async Task AddRole(RoleDTO roleDTO)
        {
            var role = _mapper.Map<Role>(roleDTO);

            await _roleRepository.AddRole(role);
        }

        public async Task UpdateRole(RoleDTO roleDTO, UpdateRoleDescriptionRequest urdr)
        {
            var role = await _roleRepository.GetRoleById(roleDTO.RoleId);

            await _roleRepository.UpdateRole(role, _mapper.Map<UpdateRoleQuery>(urdr));
        }

        public async Task DeleteRole(RoleDTO roleDTO)
        {
            var role = await _roleRepository.GetRoleById(roleDTO.RoleId);

            await _roleRepository.DeleteRole(role);
        }
    }
}