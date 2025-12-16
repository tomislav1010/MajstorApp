namespace MajstorFinder.WebAPI.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public int Iterations { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
