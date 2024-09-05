namespace GestStockAPI.Models
{
    public class InventoryResult
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int PurchaseQuantity { get; set; }
        public int SalesQuantity { get; set; }
        public int RemainingQuantity { get; set; }
    }
}
