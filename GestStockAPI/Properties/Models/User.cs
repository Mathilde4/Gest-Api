namespace GestStockAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string  PasswordHash { get; set; }
        

        public User()
    {
    }

        public User(int id, string userName, string passwordHash)
        {
            UserId = id;
            UserName = userName;
            PasswordHash = passwordHash;
            
        }
    }

}
