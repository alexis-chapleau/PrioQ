using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PrioQ.Application.Interfaces;
using PrioQ.Infrastructure.Configuration;
using PrioQ.Infrastructure.Analytics;
using PrioQ.Infrastructure.Factories;
using PrioQ.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using PrioQ.Application.UseCases;
using Microsoft.Extensions.Logging;
using PrioQ.Domain.Entities;

namespace PrioQ.Tests.UnitTests
{
    public class ProgramTests
    {
        [Fact]
        public void All_Critical_Services_Are_Registered()
        {
            // Arrange
            // Build the host as in Program.cs without running the server.
            var builder = WebApplication.CreateBuilder(new string[] { });

            // Replicate the same service registrations as in Program.cs.
            var services = builder.Services;
            services.AddSingleton<IConfigProvider>(provider =>
            {
                // For test purposes, we can point to a dummy config file or provide a stub.
                return new JsonConfigProvider("config.json");
            });
            services.AddSingleton<QueueConfig>(provider =>
            {
                var configProvider = provider.GetRequiredService<IConfigProvider>();
                return configProvider.GetQueueConfig();
            });
            services.AddSingleton<IQueueRepository, QueueRepository>();
            services.AddLogging(configure => configure.AddConsole());
            services.AddSingleton<IAnalyticsCollector, AnalyticsCollector>();
            services.AddTransient<IAnalyticsReportGenerator, AnalyticsReportGenerator>();
            services.AddTransient<IPriorityQueueDecoratorFactory, AnalyticsDecoratorFactory>();
            services.AddTransient<IPriorityQueueDecoratorFactory, LoggingDecoratorFactory>();
            services.AddTransient<IPriorityQueueDecoratorFactory, LockingDecoratorFactory>();
            services.AddTransient<IPriorityQueueDecoratorFactory, LazyDeleteDecoratorFactory>();
            services.AddSingleton<IPriorityQueueFactory, PriorityQueueFactory>();
            services.AddTransient<EnqueueCommandUseCase>();
            services.AddTransient<DequeueCommandUseCase>();
            services.AddTransient<InitializeQueueUseCase>();
            services.AddTransient<GetAnalyticsReportUseCase>();
            services.AddControllers();

            var app = builder.Build();
            var sp = app.Services;

            // Act & Assert: Verify that key services can be resolved.
            Assert.NotNull(sp.GetService<IConfigProvider>());
            Assert.NotNull(sp.GetService<IQueueRepository>());
            Assert.NotNull(sp.GetService<IAnalyticsCollector>());
            Assert.NotNull(sp.GetService<IPriorityQueueFactory>());
            Assert.NotNull(sp.GetService<EnqueueCommandUseCase>());
            Assert.NotNull(sp.GetService<DequeueCommandUseCase>());
            Assert.NotNull(sp.GetService<InitializeQueueUseCase>());
            Assert.NotNull(sp.GetService<GetAnalyticsReportUseCase>());
        }
    }
}
