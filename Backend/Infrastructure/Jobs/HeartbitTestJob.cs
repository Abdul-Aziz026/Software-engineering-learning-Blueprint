using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Jobs;

public class HeartbitTestJob : BackgroundService
{
    private readonly ILogger<HeartbitTestJob> _logger;

    public HeartbitTestJob(ILogger<HeartbitTestJob> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HeartbitTestJob is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"HeartbitTestJob is running at: {DateTime.Now.ToString("hh:mm:ss")}");
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
