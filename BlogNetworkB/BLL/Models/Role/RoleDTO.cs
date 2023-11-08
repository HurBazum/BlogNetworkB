using BlogNetworkB.BLL.Models.Author;

namespace BlogNetworkB.BLL.Models.Role
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<AuthorDTO> Authors { get; set; } = new();
    }
}