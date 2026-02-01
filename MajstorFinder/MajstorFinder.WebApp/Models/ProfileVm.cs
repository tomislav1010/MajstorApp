namespace MajstorFinder.WebApp.Models
{
    public class ProfileVm
    {
        public int Id { get; set; }
        public string Username { get; set; } = " ";
        public string Email { get; set; } = " ";
        public bool IsAdmin { get; set; }
    }
}
