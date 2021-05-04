using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fxr.BgService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fxr.BgService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _config;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DirectoryInfo d = new DirectoryInfo(_config.GetSection("Service").GetSection("FileLocation").GetSection("FourPoint").Value);

                foreach (var file in d.GetFiles("*.txt"))
                {
                    TrendData Data = JsonSerializer.Deserialize<TrendData>(File.ReadAllText(file.FullName));
                    _logger.LogInformation(file.FullName);
                    _logger.LogInformation(File.ReadAllText(file.FullName));

                    using (FxrContext ctx = new FxrContext())
                    {
                        ctx.StrategyLives.Add(new StrategyLive()
                        {
                            Name = "FivePoint",
                            Pair = Data.pair,
                            Trend = Data.trend
                        });
                        ctx.SaveChanges();
                    }

                    File.Delete(file.FullName);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
