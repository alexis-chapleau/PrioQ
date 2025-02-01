using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriorityQueueApp.Application.UseCases;
using PriorityQueueApp.Domain.Entities;
using PriorityQueueApp.Infrastructure.Configuration;
using PriorityQueueApp.Infrastructure.Factories;
using PriorityQueueApp.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Register services with the DI container.
var services = builder.Services;

// Register the configuration provider.
// (Assumes a "config.json" file is present in the output directory.)
services.AddSingleton<IConfigProvider>(provider =>
{
    string configFilePath = "config.json";
    return new JsonConfigProvider(configFilePath);
});

// Build a temporary provider to load configuration.
var tempProvider = services.BuildServiceProvider();
var configProvider = tempProvider.GetRequiredService<IConfigProvider>();
QueueConfig config = configProvider.GetQueueConfig();
services.AddSingleton(config);

// Register the queue repository as a singleton.
services.AddSingleton<IQueueRepository, QueueRepository>();

// Register all decorator factories.
services.AddTransient<IPriorityQueueDecoratorFactory, AnalyticsDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LoggingDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LockingDecoratorFactory>();
services.AddTransient<IPriorityQueueDecoratorFactory, LazyDeleteDecoratorFactory>();

// Register the PriorityQueueFactory.
services.AddSingleton<IPriorityQueueFactory, PriorityQueueFactory>();

// Initialize the queue and store it in the repository.
services.AddSingleton(provider =>
{
    var factory = provider.GetRequiredService<IPriorityQueueFactory>();
    var queue = factory.CreatePriorityQueue(config);
    var repo = provider.GetRequiredService<IQueueRepository>();
    repo.SetQueue(queue);
    return queue;
});

// Register use cases.
services.AddTransient<EnqueueCommandUseCase>();
services.AddTransient<DequeueCommandUseCase>();
services.AddTransient<ReinitializeQueueUseCase>();

// Register controllers.
services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
