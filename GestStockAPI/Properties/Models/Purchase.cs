namespace GestStockAPI.Models
{
    public class Purchase
{
    public int PurchaseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime Date { get; set; }
   // Foreign keys
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }

    // Navigation properties
    public Product Product { get; set; }
    public Category Category { get; set; }
    public Supplier Supplier { get; set; }

    public Purchase()
    {
    }

}
    public class PurchaseRequest
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime Date { get; set; }
    public string ProductName { get; set; }
    public string CatName { get; set; }
    public string SupplierName { get; set; }


    public PurchaseRequest()
    {

    }

}


}
