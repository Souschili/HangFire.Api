using HangFire.Api.Context;
using HangFire.Api.Entity;
using HangFire.Api.Services.Contract;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace HangFire.Api.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateRegularReport()
        {
            var job = new DailyReport
            {
                GeneratedAt = DateTime.UtcNow,
                Data = $"Report was generated at {DateTime.Now}",
                Status = "Done"
            };
            await _context.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        
        void IReportService.GenerateOnceReport()
        {
            throw new NotImplementedException();
        }
    }
}
