namespace GestStockAPI.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierFirstName { get; set; }
        public string SupplierAddress { get; set; }
        public int Phone { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Supplier()
    {
    }

      
    }

}
