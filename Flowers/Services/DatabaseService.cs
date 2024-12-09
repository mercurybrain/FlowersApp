using SQLite;
using Flowers.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Maui.Core.Primitives;
using System.Xml.Linq;
using System.Diagnostics;
using Flowers.Abstract;
using System.Reflection;
using Microsoft.Maui.Graphics.Platform;
using System.Text.RegularExpressions;

namespace Flowers.Services
{
    public class DatabaseService : IDataUpdatedNotifier
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            InitializeDatabaseAsync().ConfigureAwait(false);
        }

        public event EventHandler DataUpdated;

        public void NotifyDataUpdated()
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        private async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<User>();

            var admin = new User
            {
                Username = "Admin",
                Password = Encoder.ComputeHash("Admin", "SHA512", null),
                Phone = "Admin",
                AddressDefault = "Не указан",
                Access = 777
            };

            await SaveUserAsync(admin);

            await _database.CreateTableAsync<Bouquet>();
            await _database.CreateTableAsync<Models.Flowers>();
            await _database.CreateTableAsync<Order>();
            await _database.CreateTableAsync<Store>();
            await _database.CreateTableAsync<AssembledBouquets>();

            if (await _database.Table<Bouquet>().CountAsync() == 0) await SeedDataAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            // Удаление таблиц (или базы данных) при сбросе
            await _database.DropTableAsync<User>();
            await _database.DropTableAsync<Bouquet>();
            await _database.DropTableAsync<Models.Flowers>();
            await _database.DropTableAsync<Order>();
            await _database.DropTableAsync<Store>();
            await _database.DropTableAsync<AssembledBouquets>();

            await InitializeDatabaseAsync();
        }

        public async Task DropAssembledTable() {
            await _database.DropTableAsync<AssembledBouquets>();
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _database.Table<User>().ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(string username)
        {
            return await _database.Table<Order>().Where(o => o.Username == username).ToListAsync();
        }

        public Task<User> GetUserAsync(string username)
        {
            if (username.StartsWith('+') && Regex.IsMatch(username, @"^\+7\(\d{3}\)\d{3}-\d{2}-\d{2}$")) {
                return _database.Table<User>().FirstOrDefaultAsync(u => u.Phone == username);
            }
            return _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public Task<int> SaveUserAsync(User user)
        {
            return _database.InsertAsync(user);
        }

        public Task<int> UpdateUserAsync(User user)
        {
            return _database.UpdateAsync(user);
        }

        public Task<int> DeleteUserAsync(User user)
        {
            return _database.DeleteAsync(user);
        }
        public async Task<List<Bouquet>> GetAllBouquetsAsync()
        {
            return await _database.Table<Bouquet>().ToListAsync();
        }
        public async Task<List<AssembledBouquets>> GetAllAssembled() {
            return await _database.Table<AssembledBouquets>().ToListAsync();
        }
        public async Task<int> DeleteAssembled(AssembledBouquets bouquet) { 
            return await _database.DeleteAsync(bouquet);
        }
        public async Task<int> DeleteBouquet(Bouquet bouquet) { 
            return await _database.DeleteAsync(bouquet);
        }
        public async Task<List<Bouquet>> GetBouquetsByIdsAsync(List<int> ids)
        {
            return await _database.Table<Bouquet>().Where(b => ids.Contains(b.Id)).ToListAsync();
        }
        public async Task<List<AssembledBouquets>> GetAssembledBouquetsByIdsAsync(List<int> ids)
        {
            return await _database.Table<AssembledBouquets>().Where(b => ids.Contains(b.Id)).ToListAsync();
        }
        public async Task<Bouquet> GetBouquetByNameAsync(string name) { 
            return await _database.Table<Bouquet>().FirstOrDefaultAsync(b => b.Name == name);
        }
        public async Task<int> AddBouquetAsync(Bouquet bouquet)
        {
            return await _database.InsertAsync(bouquet);
        }

        public async Task<int> AddAssembledAsync(AssembledBouquets bouquet) { 
            return await _database.InsertAsync(bouquet);
        }
        public async Task<int> UpdateBouquetAsync(Bouquet bouquet)
        {
            return await _database.UpdateAsync(bouquet);
        }
        public async Task<int> UpdateStoreAsync(Store store) { 
            return await _database.UpdateAsync(store);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _database.Table<User>().ToListAsync();
        }
        public async Task<List<Models.Flowers>> GetAllFlowersAsync()
        {
            return await _database.Table<Models.Flowers>().ToListAsync();
        }
        public async Task<int> AddFlowerAsync(Models.Flowers flower) {
            return await _database.InsertAsync(flower);
        }
        public async Task<int> DeleteFlowerAsync(Models.Flowers flower)
        {
            return await _database.DeleteAsync(flower);
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _database.Table<Order>().ToListAsync();
        }
        public async Task UpdateOrderAsync(Order order)
        {
            await _database.UpdateAsync(order);
        }
        public async Task<List<Order>> GetUndeliveredOrdersAsync()
        {
            return await _database.Table<Order>()
                .Where(o => o.StatusText != "OrderDelivered")
                .ToListAsync();
        }

        public Task<int> DeleteOrderAsync(Order order)
        {
            return _database.DeleteAsync(order);
        }
        public Task<int> DeleteStoreAsync(Store store) {
            return _database.DeleteAsync(store);
        }

        public async Task<List<Store>> GetAllStoresAsync()
        {
            return await _database.Table<Store>().ToListAsync();
        }
        public async Task<Store> GetStoreById(int id)
        {
            return await _database.Table<Store>().FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<int> AddOrderAsync(Order order)
        {
            return await _database.InsertAsync(order);
        }

        public async Task<byte[]> PickImageAsync()
        {
            var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Выберите изображение для букета"
            });

            if (file != null)
            {
                using (var stream = await file.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }
        public async Task<byte[]> ConvertResImageToDBArray(string imageName)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream($"Flowers.Resources.Images.{imageName}"))
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Метод для заполнения базы данных тестовыми данными
        public async Task SeedDataAsync()
        {
            if ((await _database.Table<Bouquet>().CountAsync()) > 0)
                return;


            var stores = new List<Store>
            {
                new Store { Name = "Цветы у дома", Address = "ул. Тверская, д. 1" },
                new Store { Name = "Цветочный рай", Address = "пр. Невский, д. 45" },
                new Store { Name = "Перецвет", Address = "ул. Московская д. 9"}
            };

            await _database.InsertAllAsync(stores);
            var storesCompleted = await GetAllStoresAsync();

            var flowers = new List<Models.Flowers>
            {
                new Models.Flowers { Name = "Роза", Price = 120, StoreId = storesCompleted[0].Id },
                new Models.Flowers { Name = "Тюльпан", Price = 90, StoreId = storesCompleted[2].Id },
                new Models.Flowers { Name = "Лилия", Price = 150, StoreId = storesCompleted[1].Id },
                new Models.Flowers { Name = "Орхидея", Price = 200, StoreId = storesCompleted[1].Id },
                new Models.Flowers { Name = "Ромашка", Price = 50, StoreId = storesCompleted[0].Id},
                new Models.Flowers { Name = "Василёк", Price = 90, StoreId = storesCompleted[1].Id},
                new Models.Flowers { Name = "Пион", Price = 115, StoreId = storesCompleted[2].Id},
                new Models.Flowers { Name = "Колокольчик", Price = 50, StoreId = storesCompleted[0].Id}
            };

            var bouquet1Icon = await ConvertResImageToDBArray("bouquet_1.jpg");
            var bouquet2Icon = await ConvertResImageToDBArray("bouquet_2.jpg");
            var bouquet3Icon = await ConvertResImageToDBArray("bouquet_3.jpg");
            var bouquet4Icon = await ConvertResImageToDBArray("bouquet_4.jpg");
            var bouquet5Icon = await ConvertResImageToDBArray("bouquet_5.jpg");
            var bouquet6Icon = await ConvertResImageToDBArray("bouquet_6.jpg");
            if (bouquet2Icon == null || bouquet2Icon.Length == 0)
            {
                Trace.WriteLine("Ошибка: иконка не загружена.");
            }

            var bouquets = new List<Bouquet>
            {
                new Bouquet
                {
                    Name = "Весенний букет",
                    Price = 1200,
                    Flowers = new Dictionary<string, int>
                    {
                        { "Роза", 14 },
                        { "Тюльпан", 6 }
                    },
                    Icon = bouquet1Icon,
                    StoreId = storesCompleted[0].Id
                },
                new Bouquet
                {
                    Name = "Солнечное утро",
                    Price = 2280,
                    Flowers = new Dictionary<string, int>
                    {
                        { "Роза", 6 },
                        { "Тюльпан", 14 }
                    },
                    Icon = bouquet2Icon,
                    StoreId = storesCompleted[1].Id
                },
                new Bouquet
                { 
                    Name = "Магия Лета",
                    Price = 3500,
                    Flowers = new Dictionary<string, int>
                    {
                        {"Орхидея", 10},
                        {"Колокольчик", 2}
                    },
                    Icon = bouquet3Icon,
                    StoreId = storesCompleted[2].Id
                },
                new Bouquet
                {
                    Name = "Полет мечты",
                    Price = 3500,
                    Flowers = new Dictionary<string, int>
                    {
                        {"Орхидея", 10},
                        {"Колокольчик", 2}
                    },
                    Icon = bouquet4Icon,
                    StoreId = storesCompleted[2].Id
                },
                new Bouquet
                {
                    Name = "Зимняя сказка",
                    Price = 3500,
                    Flowers = new Dictionary<string, int>
                    {
                        {"Орхидея", 10},
                        {"Колокольчик", 2}
                    },
                    Icon = bouquet5Icon,
                    StoreId = storesCompleted[2].Id
                },
                new Bouquet
                {
                    Name = "Осенняя нежность",
                    Price = 3500,
                    Flowers = new Dictionary<string, int>
                    {
                        {"Орхидея", 10},
                        {"Колокольчик", 2}
                    },
                    Icon = bouquet6Icon,
                    StoreId = storesCompleted[2].Id
                }
            };
            await _database.InsertAllAsync(stores);
            await _database.InsertAllAsync(flowers);
            await _database.InsertAllAsync(bouquets);

            NotifyDataUpdated();
        }

    }
}
