using Flowers.Abstract;
using System;
using System.Collections.Generic;

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
}