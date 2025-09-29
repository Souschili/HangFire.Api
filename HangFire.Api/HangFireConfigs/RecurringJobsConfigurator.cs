using Hangfire;
using HangFire.Api.Services.Contract;

namespace HangFire.Api.HangFireConfigs
{
    public static class RecurringJobsConfigurator
    {

        public static void RegisterJob()
        {
            //------------------------------
            // регистрации джоба который вызывает метод сервиса каждые 10 сек
            //------------------------------
            RecurringJob.AddOrUpdate<IReportService>(
                "RegularReport",
                x => x.GenerateRegularReport(),
                "*/10 * * * * *");
        }

    }
}
