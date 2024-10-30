using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowers.Abstract
{
    public interface ISharingService
    {
        event EventHandler<object> ItemAdded;
        void Add<T>(string key, T value) where T : class;
        T GetValue<T>(string key) where T : class;
    }
}
