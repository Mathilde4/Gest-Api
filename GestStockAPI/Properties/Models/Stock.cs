namespace GestStockAPI.Models
{
    public class Stock
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Stock()
    {
    }

    public Stock(int stockId ,int productId, int quantity )
    {
        StockId = stockId;
        ProductId = productId;
        Quantity = quantity;
    }
    }
}
