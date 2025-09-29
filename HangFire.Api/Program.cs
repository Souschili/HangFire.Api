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

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(x =>
            x.UseNpgsql(builder.Configuration.GetConnectionString("Docker")));

            builder.Services.AddScoped<IReportService, ReportService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHangfire(cfg=> 
            cfg.UsePostgreSqlStorage(x=> x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Docker")),new PostgreSqlStorageOptions
            {
                SchemaName = "hangfire",                       //  Отдельная схема в Postgres для всех таблиц Hangfire
                QueuePollInterval = TimeSpan.FromSeconds(5),   //  Как часто воркеры проверяют очередь на новые джобы
                InvisibilityTimeout = TimeSpan.FromMinutes(30), // Время, в течение которого джоба считается "в работе" (если воркер упал, джоба станет видимой)
                PrepareSchemaIfNecessary = true                 //  Автоматически создаёт таблицы Hangfire, если их нет

            }));

            builder.Services.AddHangfireServer();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.UseHangfireDashboard();
            RecurringJobsConfigurator.RegisterJob();

            app.MapControllers();

            app.Run();
        }
    }
}
