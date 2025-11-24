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
            // удалит джобу если айди одинаковый ,по сути пересоздаст 
            // TODO: add constant class
            RecurringJob.RemoveIfExists("RegularJob_Daily");

            RecurringJob.AddOrUpdate<IReportService>(
              recurringJobId:"RegularJob_Daily",
              methodCall:x=> x.GenerateRegularReport(),
              cronExpression: "*/10 * * * * *",
              queue:"medium" 
            );

            RecurringJob.AddOrUpdate<IReportService>(
              recurringJobId: "RegularJob_Month",
              methodCall: x => x.GenerateOnceReportAsync(),
              cronExpression: "*/20 * * * * *",
              queue: "medium"
            );
        }

    }
}
