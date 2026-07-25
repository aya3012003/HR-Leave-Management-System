namespace Project_3.src.Application.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;

        public int Port { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool EnableSsl { get; set; }
    }
}
