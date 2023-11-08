using BlogNetworkB.BLL.Models.Role;
using ConnectionLib.DAL.Enteties;

namespace BlogNetworkB.Infrastructure.Extensions
{
    /// <summary>
    /// только для ICollection<Role>
    /// </summary>
    public static class CollectionToString
    {
        public static string ConvertToString(this ICollection<Role> ts)
        {
            string col = string.Empty;

            foreach (Role item in ts)
            {
                col += item.Name + " ";
            }

            return col;
        }

        public static string RoleToString(this ICollection<RoleDTO> ts)
        {
            string col = string.Empty;
            foreach (RoleDTO item in ts)
            {
                col += item.Name + " ";
            }
            return col;
        }
    }
}