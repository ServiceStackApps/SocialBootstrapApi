namespace SocialBootstrapApi.Models
{
    /// <summary>
    /// Custom User DataModel for harvesting UserAuth info into your own DB table
    /// </summary>   
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string TwitterUserId { get; set; }
        public string TwitterScreenName { get; set; }
        public string TwitterName { get; set; }
        public string FacebookName { get; set; }
        public string FacebookFirstName { get; set; }
        public string FacebookLastName { get; set; }
        public string FacebookUserId { get; set; }
        public string FacebookUserName { get; set; }
        public string FacebookEmail { get; set; }
        public string YahooUserId { get; set; }
        public string YahooFullName { get; set; }
        public string YahooEmail { get; set; }
        public string GravatarImageUrl64 { get; set; }
    }
}