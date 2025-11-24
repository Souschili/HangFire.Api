using Hangfire;
using HangFire.Api.Context;
using HangFire.Api.Entity;
using HangFire.Api.Services.Contract;

namespace HangFire.Api.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // будет пытаться выполниться 5 раз с указанными задержками между попытками.
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 5, 10, 20 })]
        public async Task GenerateRegularReport()
        {
            var job = new DailyReport
            {
                GeneratedAt = DateTime.UtcNow,
                Data = $"Report was generated at {DateTime.Now}",
                Status = "Done"
            };
            await Task.Delay(15000);
            await _context.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 5, 10, 20 })]
        public async Task GenerateOnceReportAsync()
        {
            var job = new MonthReport
            {
                GeneratedAt = DateTime.UtcNow,
                Data = $"Report was generated at {DateTime.Now}",
                Status = "Done"
            };
            await _context.AddAsync(job);
            await _context.SaveChangesAsync();
        }
    }
}
