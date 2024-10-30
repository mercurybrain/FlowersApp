using SQLite;

namespace Flowers.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, MaxLength(32)]
        public string Username { get; set; }

        public string Password { get; set; }

        [Unique, MaxLength(11)]
        public string Phone { get; set; }

        public string AddressDefault { get; set; }
    }
}
