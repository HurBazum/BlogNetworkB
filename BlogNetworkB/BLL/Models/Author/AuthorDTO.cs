using BlogNetworkB.BLL.Models.Role;

namespace BlogNetworkB.BLL.Models.Author
{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RoleDTO> RoleDTOs { get; set; } = new();
    }
}