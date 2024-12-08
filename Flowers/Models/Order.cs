using CommunityToolkit.Mvvm.ComponentModel;
using Flowers.Abstract;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Flowers.Models
{
    public class Order : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        
        public DateTime OrderDate { get; set; }

        [Ignore]
        public DateTime OrderDateOnly
        {
            get => OrderDate.Date;
            set
            {
                OrderDate = value.Date.Add(OrderDate.TimeOfDay);
                OnPropertyChanged();
            }
        }

        [Ignore]
        public TimeSpan OrderTime
        {
            get => OrderDate.TimeOfDay;
            set
            {
                OrderDate = OrderDate.Date.Add(value);
                OnPropertyChanged();
            }
        }

        [ForeignKey(typeof(User))]
        public string Username { get; set; } // Связь с пользователем, который сделал заказ

        [MaxLength(200)]
        public string DeliveryAddress { get; set; }

        [TextBlob("BouquetQuantitiesBlobbed")]
        public Dictionary<string, int> BouquetQuantities {
            get
            {
                var result = string.IsNullOrEmpty(BouquetQuantitiesBlobbed)
                    ? new Dictionary<string, int>()
                    : JsonConvert.DeserializeObject<Dictionary<string, int>>(BouquetQuantitiesBlobbed);

                Trace.WriteLine($"Десериализация BouquetQuantities: {JsonConvert.SerializeObject(result)}");
                return result;
            }
            set
            {
                BouquetQuantitiesBlobbed = JsonConvert.SerializeObject(value);
                Trace.WriteLine($"Сериализация BouquetQuantities: {BouquetQuantitiesBlobbed}");
            }
        } // Id букета и количество

        [Ignore]
        public Dictionary<IBouquet, int> DisplayBouquetQuantities { get; set; }


        // Общая стоимость заказа
        public float TotalPrice { get; set; }

        public string BouquetQuantitiesBlobbed { get; set; }

        // Строка для хранения статуса в SQLite
        public string StatusText { get; set; } = OrderStatus.OrderAccepted.ToString();

        [Ignore]
        public OrderStatus Status
        {
            get => Enum.TryParse(StatusText, out OrderStatus status) ? status : OrderStatus.Error;
            set => StatusText = value.ToString();
        }

        [Ignore]
        public string StatusDescription => Status.GetDescription();

        // Поле для связи с курьером (пользователем, который доставляет заказ)
        [ForeignKey(typeof(User))]
        public string CourierUsername { get; set; } = "Admin"; // Имя пользователя-курьера

        [Ignore]
        public User Courier { get; set; } // Объект пользователя-курьера, если необходимо

        [Ignore]
        public User Customer { get; set; } // Объект пользователя, если необходимо

        public string Description { get; set; } = "";
    }

    public enum OrderStatus
    {
        [Description("Заказ принят")]
        OrderAccepted,

        [Description("Заказ собран")]
        OrderAssembled,

        [Description("Курьер в пути")]
        CourierOnTheWay,

        [Description("Заказ доставлен")]
        OrderDelivered,

        [Description("Ошибка!")]
        Error
    }
}
