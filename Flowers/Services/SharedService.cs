using Flowers.Abstract;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Flowers.Services
{
    public class SharedService : ISharingService
    {
        private Dictionary<string, object> DTODict { get; set; } = new Dictionary<string, object>();
        public event EventHandler<object> ItemAdded;

        public void Add<T>(string key, T value) where T : class
        {
            if (DTODict.ContainsKey(key))
            {
                DTODict[key] = value;
            }
            else
            {
                DTODict.Add(key, value);
            }
            ItemAdded?.Invoke(this, value);
        }

        public T GetValue<T>(string key) where T : class
        {
            if (DTODict.ContainsKey(key))
            {
                return DTODict[key] as T;
            }
            return null;
        }
    }

    public class ProjectTools {
        public static (string CityStreetHouse, string? Floor, string? Apartment) ParseAddress(string addr)
        {
            if (string.IsNullOrWhiteSpace(addr) || addr == "Не указан")
                return ("", "", "");

            string cityStreetHousePattern = @"^(.*?),\s*подъезд и этаж:"; // Всё до "подъезд и этаж:"
            string floorPattern = @"подъезд и этаж:\s*([\d,\.]+)(?=,|$)"; // Число или дробь до запятой или конца строки
            string apartmentPattern = @"квартира:\s*(\d+)"; // Число после "квартира:"

            var cityStreetHouseMatch = Regex.Match(addr, cityStreetHousePattern);
            string cityStreetHouse = cityStreetHouseMatch.Success ? cityStreetHouseMatch.Groups[1].Value.Trim() : addr;

            var floorMatch = Regex.Match(addr, floorPattern);
            string? floor = floorMatch.Success ? floorMatch.Groups[1].Value.Trim() : null;

            var apartmentMatch = Regex.Match(addr, apartmentPattern);
            string? apartment = apartmentMatch.Success ? apartmentMatch.Groups[1].Value.Trim() : null;

            return (cityStreetHouse, floor, apartment);
        }
        public static string FormatAddress(string cityStreetHouse, string? floor, string? apartment)
        {
            if (string.IsNullOrWhiteSpace(cityStreetHouse))
                throw new ArgumentException("Адрес не указан!", nameof(cityStreetHouse));

            string address = cityStreetHouse.Trim();
            if (!string.IsNullOrWhiteSpace(floor))
            {
                address += $", подъезд и этаж: {floor.Trim()}";
            }
            if (!string.IsNullOrWhiteSpace(apartment))
            {
                address += $", квартира: {apartment.Trim()}";
            }

            return address;
        }
        public static int GenerateRandomNumber()
        {
            Random random = new Random();
            int number;

            do
            {
                number = random.Next(100, 1000);
            } while (number == 777 || number == 555);

            return number;
        }
    }
}