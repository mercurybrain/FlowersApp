using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flowers.Models
{
    public class Flowers
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        public float Price { get; set; }

        [ForeignKey(typeof(Store))]
        public int StoreId { get; set; }
    }
}
