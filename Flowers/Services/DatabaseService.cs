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
            await _database.CreateTableAsync<Bouquet>();
            await _database.CreateTableAsync<Models.Flowers>();
            await _database.CreateTableAsync<Order>();
        }

        public async Task ResetDatabaseAsync()
        {
            // Удаление таблиц (или базы данных) при сбросе
            await _database.DropTableAsync<User>();
            await _database.DropTableAsync<Bouquet>();
            await _database.DropTableAsync<Models.Flowers>();
            await _database.DropTableAsync<Order>();

            await InitializeDatabaseAsync();
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _database.Table<User>().ToListAsync();
        }

        public Task<User> GetUserAsync(string username)
        {
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
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _database.Table<User>().ToListAsync();
        }
        public async Task<List<Models.Flowers>> GetAllFlowersAsync()
        {
            return await _database.Table<Models.Flowers>().ToListAsync();
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _database.Table<Order>().ToListAsync();
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

            var flowers = new List<Models.Flowers>
            {
                new Models.Flowers {Name = "Роза", Price = 120},
                new Models.Flowers {Name = "Тюльпан", Price = 90}
            };

            var bouquet1Icon = await ConvertResImageToDBArray("bouquet_1.jpg");
            var bouquet2Icon = await ConvertResImageToDBArray("bouquet_2.jpg");
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
                    Icon = bouquet1Icon
                },
                new Bouquet
                {
                    Name = "Осенний букет",
                    Price = 2280,
                    Flowers = new Dictionary<string, int>
                    {
                        { "Роза", 6 },
                        { "Тюльпан", 14 }
                    },
                    Icon = bouquet2Icon
                }
            };

            await _database.InsertAllAsync(flowers);
            await _database.InsertAllAsync(bouquets);

            NotifyDataUpdated();
        }

    }
}
