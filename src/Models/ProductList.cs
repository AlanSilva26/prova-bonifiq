namespace ProvaPub.Models
{
    public class ProductList
    {
        public List<Product> Products { get; set; } = new List<Product>();

        public int TotalCount { get; set; }

        public bool HasNext { get; set; }
    }
}
