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

        public CartItem(Bouquet bouquet, int quantity)
        {
            SelectedBouquet = bouquet;
            Quantity = quantity;
        }
    }
    public partial class CartAssembled : ObservableObject
    {
        public AssembledBouquets SelectedBouquet { get; set; }

        [ObservableProperty]
        public int quantity;

        public CartAssembled(AssembledBouquets assembledBouquet, int quantity)
        {
            SelectedBouquet = assembledBouquet;
            Quantity = quantity;
        }
    }
}
