namespace GestStockAPI.Models
{
    public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public int CategoryId { get; set; }

    public Category Category { get; set; }
  

    public Product()
    {
    }   
}

public class ProductRequest
{
   public int ProductId { get; set; } 
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string CatName { get; set; }
    public decimal Price { get; set; }

    public ProductRequest()
    {
        
    }

}

}
