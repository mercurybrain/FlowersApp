using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowers.Models
{
    public partial class CartItem : ObservableObject
    {
        public Bouquet SelectedBouquet { get; set; }

        [ObservableProperty]
        public int quantity;

        public CartItem(Bouquet bouquet, int q) { 
            this.SelectedBouquet = bouquet;
            this.Quantity = q;
        }
    }
}
