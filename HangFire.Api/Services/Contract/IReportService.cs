﻿namespace HangFire.Api.Services.Contract
{
    public interface IReportService
    {
        Task GenerateRegularReport();
        void GenerateOnceReport();
    }
}
