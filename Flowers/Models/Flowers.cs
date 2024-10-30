using SQLite;

namespace Flowers.Models
{
    public class Flowers
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, MaxLength(32)]
        public string Name { get; set; }

        public float Price { get; set; }
    }
}
