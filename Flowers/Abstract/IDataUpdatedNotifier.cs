using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowers.Abstract
{
    interface IDataUpdatedNotifier
    {
        event EventHandler DataUpdated;
        void NotifyDataUpdated();
    }
}
