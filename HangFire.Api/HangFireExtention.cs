using HangFire.Api.Services.Contract;
using HangFire.Api.Services;
using Hangfire;
using Hangfire.PostgreSql;
using HangFire.Api.HangFireConfigs;

namespace HangFire.Api
{
    //----------------------------
    // использовать так ,в програм файле добавляем 
    // builder.Services.AddHangfireWithJobs(builder.Configuration);
    // app.UseHangfireJobs();
    //----------------------------
    public static class HangFireExtention
    {
        public static IServiceCollection AddHangfireWithJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // ----------------------------
            // Hangfire: подключение к Postgres
            // ----------------------------
            services.AddHangfire(cfg =>
                cfg.UsePostgreSqlStorage(
                    x => x.UseNpgsqlConnection(configuration.GetConnectionString("Docker")),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",
                        QueuePollInterval = TimeSpan.FromSeconds(5),
                        InvisibilityTimeout = TimeSpan.FromMinutes(30),
                        PrepareSchemaIfNecessary = true
                    }));

            // ----------------------------
            // Hangfire сервер (воркеры)
            // ----------------------------
            services.AddHangfireServer();

            // ----------------------------
            // Регистрация всех сервисов, которые будут использоваться в джобах
            // ----------------------------
            services.AddScoped<IReportService, ReportService>();

            return services;
        }

        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
        {
            // ----------------------------
            // Dashboard
            // ----------------------------
            app.UseHangfireDashboard();

            // ----------------------------
            // Регистрация всех регулярных джоб
            // ----------------------------
            RecurringJobsConfigurator.RegisterJob();

            return app;
        }

    }
}
