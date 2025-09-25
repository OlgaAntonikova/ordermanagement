namespace OrderManagement.Api.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long TelegramId { get; set; }        
        public string? Username { get; set; }
        public long? ChatId { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
