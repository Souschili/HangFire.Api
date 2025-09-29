using Hangfire;
using HangFire.Api.Services.Contract;

namespace HangFire.Api.HangFireConfigs
{
    public static class RecurringJobsConfigurator
    {

        public static void RegisterJob()
        {
           
            RecurringJob.AddOrUpdate<IReportService>("RegularReport",
                x=> x.GenerateRegularReport(),
                "*/10 * * * * *"); // cron-синтаксис 10 сек
        }

    }
}
