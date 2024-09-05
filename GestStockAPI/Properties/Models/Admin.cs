namespace GestStockAPI.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string  PasswordHash { get; set; }
        
        

        public Admin()
    {
    }

        public Admin(int id, string adminName, string passwordHash)
        {
            AdminId = id;
            AdminName = adminName;
            PasswordHash = passwordHash;
            
        }
    }

}
