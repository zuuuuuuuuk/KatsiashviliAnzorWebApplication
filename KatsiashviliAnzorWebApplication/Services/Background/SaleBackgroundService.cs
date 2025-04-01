using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class SaleBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SaleBackgroundService> _logger;

        public SaleBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SaleBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _logger.LogInformation("SaleBackgroundService instance created.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sale Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Always use a single UTC time reference point for all comparisons
                    DateTime currentUtcTime = DateTime.UtcNow;
                    _logger.LogInformation($"Background service check at {currentUtcTime:yyyy-MM-dd HH:mm:ss} UTC");

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var saleService = scope.ServiceProvider.GetRequiredService<ISaleService>();
                        var sales = saleService.GetAllSales()
                            .Where(s => s.StartsAt.HasValue && s.EndsAt.HasValue)
                            .ToList();

                        _logger.LogInformation($"Found {sales.Count} sales to process.");

                        foreach (var sale in sales)
                        {
                            // Clear logging with explicit UTC markers
                            _logger.LogInformation(
                                $"Processing sale ID {sale.Id}: IsActive={sale.IsActive}, " +
                                $"StartsAt={sale.StartsAt?.ToString("yyyy-MM-dd HH:mm:ss")} UTC, " +
                                $"EndsAt={sale.EndsAt?.ToString("yyyy-MM-dd HH:mm:ss")} UTC, " +
                                $"Now={currentUtcTime:yyyy-MM-dd HH:mm:ss} UTC");

                            // Deactivate expired sales
                            if (sale.IsActive && sale.EndsAt < currentUtcTime)
                            {
                                saleService.DeactivateSale(sale.Id);
                                _logger.LogInformation($"Deactivated sale with ID {sale.Id}");
                            }
                            // Activate sales that should be active now
                            else if (!sale.IsActive &&
                                     sale.StartsAt <= currentUtcTime &&
                                     sale.EndsAt >= currentUtcTime)
                            {
                                saleService.ActivateSaleWithDefaultDates(sale.Id);
                                _logger.LogInformation($"Activated sale with ID {sale.Id}");
                            }
                            else
                            {
                                _logger.LogInformation($"No action needed for sale ID {sale.Id}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in background service: {ex.Message}");
                }

                // Wait before next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}