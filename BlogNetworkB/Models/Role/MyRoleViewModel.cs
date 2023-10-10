namespace BlogNetworkB.Models.Role
{
    /// <summary>
    /// для не админов,
    /// т.к. решил брать имена ролей не из бд,
    /// а из User.Claims
    /// </summary>
    public class MyRoleViewModel
    {
        public ICollection<string> Roles { get; set; } = null!;
    }
}