using SQLite;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;
using Flowers.Abstract;

namespace Flowers.Models
{
    public class AssembledBouquets : IBouquet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        public float Price { get; set; }

        public byte[] Icon { get; set; }

        [ForeignKey(typeof(Store))]
        public int StoreId { get; set; }

        [Ignore]
        public Store Store { get; set; }

        // Строка для хранения цветов и их количества в формате JSON
        public string FlowerData { get; set; }

        // Поле для удобного использования цветов в коде
        [Ignore]
        public Dictionary<string, int> Flowers
        {
            get
            {
                return string.IsNullOrEmpty(FlowerData)
                    ? new Dictionary<string, int>()
                    : JsonConvert.DeserializeObject<Dictionary<string, int>>(FlowerData);
            }
            set
            {
                FlowerData = JsonConvert.SerializeObject(value);
            }
        }
    }
}
