namespace Microbiology.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public byte[] PasswordHash { get; set; } // Stores hashed password
        public byte[] PasswordSalt { get; set; } // Stores unique salt
    }
}
