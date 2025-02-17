﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrioQ.Application.UseCases;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Configuration;
using PrioQ.Infrastructure.Factories;
using PrioQ.Infrastructure.Repository;
using PrioQ.Infrastructure.Analytics;
using PrioQ.Presentation.Filters;
using PrioQ.Infrastructure.Decorators; // if you still use decorators

var builder = WebApplication.CreateBuilder(args);

// Register services with the DI container.
var services = builder.Services;

// Register the configuration provider.
services.AddSingleton<IConfigProvider>(provider =>
{
    string configFilePath = "config.json";
    return new JsonConfigProvider(configFilePath);
});

// Register QueueConfig by using the configuration provider.
services.AddSingleton<QueueConfig>(provider =>
{
    var configProvider = provider.GetRequiredService<IConfigProvider>();
    return configProvider.GetQueueConfig();
});

// Register the queue repository as a singleton.
services.AddSingleton<IQueueRepository, QueueRepository>();

// Register logging (this makes ILogger<T> available)
services.AddLogging(configure => configure.AddConsole());

// Register the analytics collector as a singleton so that all data is accumulated.
services.AddSingleton<IAnalyticsCollector, AnalyticsCollector>();

// Register the report generator and its interface.
services.AddTransient<IAnalyticsReportGenerator, AnalyticsReportGenerator>();

// Register all decorator factories.
services.AddTransient<IPriorityQueueDecoratorFactory, AnalyticsDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LoggingDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LockingDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LazyDeleteDecoratorFactory>();

// Register the PriorityQueueFactory.
services.AddSingleton<IPriorityQueueFactory, PriorityQueueFactory>();

// Register use cases.
services.AddTransient<IEnqueueCommandUseCase, EnqueueCommandUseCase>();
services.AddTransient<IDequeueCommandUseCase, DequeueCommandUseCase>();
services.AddTransient<IInitializeQueueUseCase, InitializeQueueUseCase>();
services.AddTransient<IGetAnalyticsReportUseCase, GetAnalyticsReportUseCase>();

// Register controllers with views and add the global exception filter.
services.AddControllersWithViews(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
});

var app = builder.Build();

// Conditionally create the queue only if we are in "Testing" environment.
if (builder.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var provider = scope.ServiceProvider;
        var configProvider = provider.GetRequiredService<IConfigProvider>();
        QueueConfig config = configProvider.GetQueueConfig();
        var factory = provider.GetRequiredService<IPriorityQueueFactory>();
        var queue = factory.CreatePriorityQueue(config);
        var repo = provider.GetRequiredService<IQueueRepository>();
        repo.SetQueue(queue);
        Console.WriteLine("Queue created automatically in Testing environment.");
    }
}

app.UseRouting();
app.UseStaticFiles(); // Enables serving files from wwwroot/
app.UseAuthorization();

app.MapControllers();  // For attribute-routed controllers, if needed
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose the Program class for testing purposes.
public partial class Program { }
