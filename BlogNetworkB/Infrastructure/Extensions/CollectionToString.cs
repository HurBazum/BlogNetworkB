using ConnectionLib.DAL.Enteties;

namespace BlogNetworkB.Infrastructure.Extensions
{
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
    }
}