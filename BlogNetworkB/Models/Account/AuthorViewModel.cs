namespace BlogNetworkB.Models.Account
{
    public class AuthorViewModel : RegisterViewModel
    {
        public int CommentsCount { get; set; }
        public int ArticlesCount { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string GetFullName() => FirstName + " " + LastName;
    }
}