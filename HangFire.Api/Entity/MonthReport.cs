namespace HangFire.Api.Entity
{
    public class MonthReport
    {
        public int Id { get; set; }                 // PK
        public DateTime GeneratedAt { get; set; }   // Когда джоба выполнилась
        public string Data { get; set; } = "";      // Любые данные отчёта
        public string Status { get; set; } = "OK";  // Статус выполнения (OK / Failed)
    }
}
