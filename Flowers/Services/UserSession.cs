using Flowers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowers.Services
{
    class UserSession
    {
        private static UserSession _instance;
        private UserSession() { }

        // Метод для получения единственного экземпляра
        public static UserSession Instance => _instance ??= new UserSession();
        public User CurrentUser { get; set; }
    }
}
