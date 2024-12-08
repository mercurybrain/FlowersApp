

using Flowers.Models;

namespace Flowers.Abstract
{
    public interface IBouquet
    {
        int Id { get; }
        string Name { get; }
        float Price { get; }
        int StoreId { get; set; }
        Store Store { get; set; }
    }
}
