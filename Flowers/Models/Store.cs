using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowers.Models
{
    public class Store
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Models.Flowers> Flowers { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Bouquet> Bouquets { get; set; }
    }
}
