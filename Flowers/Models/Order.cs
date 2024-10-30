using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Flowers.Models
{
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        [ForeignKey(typeof(User))]
        public string Username { get; set; }

        [MaxLength(200)]
        public string DeliveryAddress { get; set; }

        // Словарь для хранения букетов и их количества
        [TextBlob("BouquetQuantitiesBlobbed")]
        public Dictionary<Bouquet, int> BouquetQuantities { get; set; }

        // Общая стоимость заказа
        public float TotalPrice { get; set; }

        // Поле для хранения сериализованного словаря BouquetQuantities
        public string BouquetQuantitiesBlobbed { get; set; }

        // Строка для хранения статуса в SQLite
        public string StatusText { get; set; } = OrderStatus.OrderAccepted.ToString();

        // Не сохраняемое поле, используемое в коде
        [Ignore]
        public OrderStatus Status
        {
            get => Enum.TryParse(StatusText, out OrderStatus status) ? status : OrderStatus.Error;
            set => StatusText = value.ToString();
        }
    }

    public enum OrderStatus
    {
        OrderAccepted,   // Заказ принят
        OrderAssembled,  // Заказ собран
        CourierOnTheWay, // Курьер в пути
        OrderDelivered,  // Заказ доставлен
        Error            // Ошибка!
    }
}
