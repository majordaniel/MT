
namespace MTMiddleware.Shared.Models
{
    public class EmailSettings
    {
        public string To { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
    }
}
