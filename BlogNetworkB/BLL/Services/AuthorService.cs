using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Models.Author;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using AutoMapper;
using BlogNetworkB.BLL.Models.Role;
using ConnectionLib.DAL.Queries.Author;

namespace BlogNetworkB.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        readonly IMapper _mapper;
        readonly IAuthorRepository _authorRepository;
        readonly IRoleRepository _roleRepository;

        public AuthorService(IAuthorRepository authorRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<AuthorDTO[]> AuthorDTOlist()
        {
            var authors = await _authorRepository.GetAll();
            
            return _mapper.Map<AuthorDTO[]>(authors);
        }

        public async Task AddAuthor(AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);

            foreach (var role in authorDTO.RoleDTOs)
            {
                var r = await _roleRepository.GetRoleById(role.RoleId);
                author.Roles.Add(r);
            }

            await _authorRepository.AddAuthor(author);
        }

        public async Task<AuthorDTO> GetAuthorDTOByEmail(string email)
        {
            var author = await _authorRepository.GetAuthorByEmail(email);

            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task<bool> AuthorExists(string email)
        {
            if(await _authorRepository.GetAuthorByEmail(email) != null)
            {
                return true;
            }

            return false;
        }

        public async Task<RoleDTO[]> GetAuthorRoleDTOs(AuthorDTO authorDTO)
        {
            var author = await _authorRepository.GetAuthorByEmail(authorDTO.Email);
            var roles = await _authorRepository.GetAuthorsRoles(author);

            return _mapper.Map<RoleDTO[]>(roles);
        }

        public async Task<AuthorDTO> GetAuthorDTOById(int id)
        { 
            var author = await _authorRepository.GetAuthorById(id);
            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task DeleteAuthor(AuthorDTO authorDTO)
        {
            var author = await _authorRepository.GetAuthorByEmail(authorDTO.Email);

            await _authorRepository.DeleteAuthor(author);
        }

        public async Task UpdateAuthor(AuthorDTO authorDTO, UpdateAuthorRequest uar)
        {
            var author = await _authorRepository.GetAuthorByEmail(authorDTO.Email);

            await _authorRepository.UpdateAuthor(author, _mapper.Map<UpdateAuthorQuery>(uar));
        }

        public async Task<AuthorDTO> GetCurrentAuthorDTO(HttpContext context)
        {
            var author = await _authorRepository.GetAuthorByEmail(context.User.Claims.FirstOrDefault().Value);

            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task AddRoleToAuthor(AuthorDTO authorDTO, RoleDTO roleDTO)
        {
            var author = await _authorRepository.GetAuthorById(authorDTO.AuthorId);
            var role = await _roleRepository.GetRoleById(roleDTO.RoleId);

            await _authorRepository.AddRole(author, role);
        }

        public async Task DeleteAuthorRole(AuthorDTO authorDTO, RoleDTO roleDTO)
        {
            var author = await _authorRepository.GetAuthorById(authorDTO.AuthorId);
            var role = await _roleRepository.GetRoleById(roleDTO.RoleId);

            await _authorRepository.DeleteAuthorRole(author, role);
        }
    }
}