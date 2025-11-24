using Hangfire;
using Hangfire.PostgreSql;
using HangFire.Api.Context;
using HangFire.Api.HangFireConfigs;
using HangFire.Api.Services;
using HangFire.Api.Services.Contract;
using Microsoft.EntityFrameworkCore;

namespace HangFire.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ----------------------------
            // Ќастройка DbContext дл€ работы с Postgres
            // ----------------------------
            builder.Services.AddDbContext<AppDbContext>(x =>
                x.UseNpgsql(builder.Configuration.GetConnectionString("Docker")));

            // ----------------------------
            // –егистраци€ сервиса, который будет использоватьс€ в джобах
            // ----------------------------
            builder.Services.AddScoped<IReportService, ReportService>();

            // ----------------------------
            // ƒобавление контроллеров и Swagger
            // ----------------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ----------------------------
            // Ќастройка Hangfire
            // ----------------------------
            builder.Services.AddHangfire(cfg =>
                cfg.UseSimpleAssemblyNameTypeSerializer() // —ериализатор с упрощЄнным именем сборки
                .UseRecommendedSerializerSettings()       // –екомендуемые настройки сериализации
                .UsePostgreSqlStorage(
                    // ѕередаем подключение к базе Postgres
                    x => x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Docker")),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",                          // string Ч схема дл€ всех таблиц Hangfire
                        QueuePollInterval = TimeSpan.FromSeconds(15),     // TimeSpan Ч интервал проверки очереди на новые задачи
                        InvisibilityTimeout = TimeSpan.FromMinutes(30),  // TimeSpan Ч сколько времени задача считаетс€ "зан€той" воркером
                        //DistributedLockTimeout = TimeSpan.FromMinutes(1),// TimeSpan Ч таймаут распределЄнной блокировки задачи
                        PrepareSchemaIfNecessary = true,                 // bool Ч авто-создание таблиц, если их нет
                        //JobExpirationCheckInterval = TimeSpan.FromHours(1), // TimeSpan Ч интервал проверки устаревших задач
                        //CountersAggregateInterval = TimeSpan.FromMinutes(5), // TimeSpan Ч интервал агрегации счЄтчиков дл€ Dashboard
                        //AllowUnsafeValues = true,                        // bool Ч разрешить небезопасные значени€ (например, большие строки)
                        //DeleteExpiredBatchSize = 10,                     // int Ч количество джобов, удал€емых за один проход
                        //EnableLongPolling = true,                        // bool Ч использовать long polling дл€ ускорени€ обработки задач
                        //EnableTransactionScopeEnlistment = true,        // bool Ч включить автоматическое участие в TransactionScope
                        //TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5), // TimeSpan Ч таймаут дл€ TransactionScope
                        //UseNativeDatabaseTransactions = true,           // bool Ч использовать нативные транзакции PostgreSQL
                        //UseSlidingInvisibilityTimeout = true             // bool Ч использовать "скольз€щий" InvisibilityTimeout (обновление таймаута по мере работы джобы)

                    }));


            // ----------------------------
            // «апуск Hangfire сервера (воркеры)
            // ----------------------------
            builder.Services.AddHangfireServer(opt=> new BackgroundJobServerOptions
            {
                // именованные очереди ,приоритет с лева направо
                // как только джобы в первом закончатс€ начнут выполн€тс€ джобы во втором ,а потом в третьем
                Queues = ["high", "medium", "low"]
                
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 3,              // сколько раз пытатьс€ повторно выполнить упавшую джобу
                DelaysInSeconds = new[] { 10, 30, 60 }, // интервалы между попытками
                LogEvents = true            // логировать попытки
            });

            var app = builder.Build();

            // ----------------------------
            // Swagger дл€ разработки
            // ----------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // ----------------------------
            // Hangfire Dashboard
            // ƒоступно по /hangfire, дл€ мониторинга джоб, воркеров и истории выполнени€
            // ----------------------------
            app.UseHangfireDashboard();

            // ----------------------------
            // –егистраци€ регул€рных джоб
            // ----------------------------
            RecurringJobsConfigurator.RegisterJob();

            // ----------------------------
            //  онтроллеры
            // ----------------------------
            app.MapControllers();

            app.Run();
        }
    }

}
