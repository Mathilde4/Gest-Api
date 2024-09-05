namespace GestStockAPI.Models
{
    public class Sale
    {
    public int SaleId { get; set; }  // Primary key
    public int ProductId { get; set; }  // Foreign key to Product
    public int CategoryId { get; set; }  // Foreign key to Category
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime Date { get; set; }
    public string ClientName { get; set; }
    public int ReceiptNumber {get; set; }

    // Navigation properties
    public Product Product { get; set; }
    public Category Category { get; set; }

    // Computed properties (not stored in the database)
    public string ProductName => Product?.ProductName;
    public string CatName => Category?.CatName;
    public decimal Total => Quantity * UnitPrice;

        public Sale()
        {
        }
    }

    public class SaleRequest
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public string CatName { get; set; }
        public string ClientName { get; set; }
        

        public SaleRequest()
        {
        }
    }
}
