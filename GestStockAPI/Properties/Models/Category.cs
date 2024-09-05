namespace GestStockAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CatName { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Category() { }

        public Category(int categoryID, string name)
        {
            CategoryId = categoryID;
            CatName = name;
        }
    }
}
