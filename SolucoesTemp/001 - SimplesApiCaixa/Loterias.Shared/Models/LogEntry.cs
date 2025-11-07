namespace Loterias.Shared.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Service { get; set; } = string.Empty;
        public string Level { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
        public string? Exception { get; set; }
        public string TraceId { get; set; } = Guid.NewGuid().ToString();
    }
}
