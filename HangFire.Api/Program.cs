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
            // ��������� DbContext ��� ������ � Postgres
            // ----------------------------
            builder.Services.AddDbContext<AppDbContext>(x =>
                x.UseNpgsql(builder.Configuration.GetConnectionString("Docker")));

            // ----------------------------
            // ����������� �������, ������� ����� �������������� � ������
            // ----------------------------
            builder.Services.AddScoped<IReportService, ReportService>();

            // ----------------------------
            // ���������� ������������ � Swagger
            // ----------------------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ----------------------------
            // ��������� Hangfire
            // ----------------------------
            builder.Services.AddHangfire(cfg =>
                cfg.UsePostgreSqlStorage(
                    // �������� ����������� � ���� Postgres
                    x => x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Docker")),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",                          // string � ����� ��� ���� ������ Hangfire
                        QueuePollInterval = TimeSpan.FromSeconds(15),     // TimeSpan � �������� �������� ������� �� ����� ������
                        InvisibilityTimeout = TimeSpan.FromMinutes(30),  // TimeSpan � ������� ������� ������ ��������� "�������" ��������
                        //DistributedLockTimeout = TimeSpan.FromMinutes(1),// TimeSpan � ������� ������������� ���������� ������
                        PrepareSchemaIfNecessary = true,                 // bool � ����-�������� ������, ���� �� ���
                        //JobExpirationCheckInterval = TimeSpan.FromHours(1), // TimeSpan � �������� �������� ���������� �����
                        //CountersAggregateInterval = TimeSpan.FromMinutes(5), // TimeSpan � �������� ��������� ��������� ��� Dashboard
                        //AllowUnsafeValues = true,                        // bool � ��������� ������������ �������� (��������, ������� ������)
                        //DeleteExpiredBatchSize = 10,                     // int � ���������� ������, ��������� �� ���� ������
                        //EnableLongPolling = true,                        // bool � ������������ long polling ��� ��������� ��������� �����
                        //EnableTransactionScopeEnlistment = true,        // bool � �������� �������������� ������� � TransactionScope
                        //TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5), // TimeSpan � ������� ��� TransactionScope
                        //UseNativeDatabaseTransactions = true,           // bool � ������������ �������� ���������� PostgreSQL
                        //UseSlidingInvisibilityTimeout = true             // bool � ������������ "����������" InvisibilityTimeout (���������� �������� �� ���� ������ �����)

                    }));


            // ----------------------------
            // ������ Hangfire ������� (�������)
            // ----------------------------
            builder.Services.AddHangfireServer(opt=> new BackgroundJobServerOptions
            {
                // ����������� ������� ,��������� � ���� �������
                // ��� ������ ����� � ������ ���������� ������ ���������� ����� �� ������ ,� ����� � �������
                Queues = ["High", "Medium", "Low"]
            });

            var app = builder.Build();

            // ----------------------------
            // Swagger ��� ����������
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
            // �������� �� /hangfire, ��� ����������� ����, �������� � ������� ����������
            // ----------------------------
            app.UseHangfireDashboard();

            // ----------------------------
            // ����������� ���������� ����
            // ----------------------------
            RecurringJobsConfigurator.RegisterJob();

            // ----------------------------
            // �����������
            // ----------------------------
            app.MapControllers();

            app.Run();
        }
    }

}
