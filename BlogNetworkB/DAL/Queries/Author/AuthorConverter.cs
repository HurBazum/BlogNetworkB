namespace BlogNetworkB.DAL.Queries.Author
{
    public static class AuthorConverter
    {
        public static Enteties.Author Convert(Enteties.Author a, UpdateAuthorQuery uaq)
        {
            a.LastName = (!string.IsNullOrEmpty(uaq.NewLastName)) ? uaq.NewLastName : a.LastName;
            a.FirstName = (!string.IsNullOrEmpty(uaq.NewFirstName)) ? uaq.NewFirstName : a.FirstName;
            a.Login = (!string.IsNullOrEmpty(uaq.NewLogin)) ? uaq.NewLogin : a.Login;
            a.Email = (!string.IsNullOrEmpty(uaq.NewEmail)) ? uaq.NewEmail : a.Email;
            a.Password = (!string.IsNullOrEmpty(uaq.NewPassword)) ? uaq.NewPassword : a.Password;
            
            return a;
        }
    }
}