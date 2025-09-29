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
            // Настройка DbContext для работы с Postgres
            // ----------------------------
            builder.Services.AddDbContext<AppDbContext>(x =>
                x.UseNpgsql(builder.Configuration.GetConnectionString("Docker")));

            // ----------------------------
            // Регистрация сервиса, который будет использоваться в джобах
            // ----------------------------
            builder.Services.AddScoped<IReportService, ReportService>();

            // ----------------------------
            // Добавление контроллеров и Swagger
            // ----------------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ----------------------------
            // Настройка Hangfire
            // ----------------------------
            builder.Services.AddHangfire(cfg =>
                cfg.UsePostgreSqlStorage(
                    // Передаем подключение к базе Postgres
                    x => x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Docker")),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",                       //  Отдельная схема для всех таблиц Hangfire (чтобы не смешивать с бизнес-таблицами)
                        QueuePollInterval = TimeSpan.FromSeconds(5),   //Интервал проверки очереди на новые джобы (воркерами)
                        InvisibilityTimeout = TimeSpan.FromMinutes(30), //  Время, в течение которого джоба считается "занятой" воркером
                        PrepareSchemaIfNecessary = true               //  Автоматически создаёт таблицы Hangfire, если их нет
                                                                      // Параметр DisableGlobalLocks больше не нужен — оптимизация включена по умолчанию
                    }));

            // ----------------------------
            // Запуск Hangfire сервера (воркеры)
            // ----------------------------
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // ----------------------------
            // Swagger для разработки
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
            // Доступно по /hangfire, для мониторинга джоб, воркеров и истории выполнения
            // ----------------------------
            app.UseHangfireDashboard();

            // ----------------------------
            // Регистрация регулярных джоб
            // ----------------------------
            RecurringJobsConfigurator.RegisterJob();

            // ----------------------------
            // Контроллеры
            // ----------------------------
            app.MapControllers();

            app.Run();
        }
    }

}
