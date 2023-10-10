namespace BlogNetworkB.BLL.Models.Author
{
    public class UpdateAuthorRequest
    {
        public string? NewFirstName { get; set; }
        public string? NewLastName { get; set; }
        public string? NewLogin { get; set; }
        public string? NewPassword { get; set; }
        public string? NewEmail { get; set; }
    }
}