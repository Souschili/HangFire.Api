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
                        SchemaName = "hangfire",                       //  ��������� ����� ��� ���� ������ Hangfire (����� �� ��������� � ������-���������)
                        QueuePollInterval = TimeSpan.FromSeconds(5),   //�������� �������� ������� �� ����� ����� (���������)
                        InvisibilityTimeout = TimeSpan.FromMinutes(30), //  �����, � ������� �������� ����� ��������� "�������" ��������
                        PrepareSchemaIfNecessary = true               //  ������������� ������ ������� Hangfire, ���� �� ���
                                                                      // �������� DisableGlobalLocks ������ �� ����� � ����������� �������� �� ���������
                    }));

            // ----------------------------
            // ������ Hangfire ������� (�������)
            // ----------------------------
            builder.Services.AddHangfireServer();

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
