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
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var saleService = scope.ServiceProvider.GetRequiredService<ISaleService>();

                        var sales = saleService.GetAllSales()
                            .Where(s => s.StartsAt.HasValue && s.EndsAt.HasValue)
                            .ToList();


                        _logger.LogInformation($"Found {sales.Count} sales to process.");


                        foreach (var sale in sales)
                        {
                            // Check if the sale is active and should be deactivated
                            _logger.LogInformation($"Processing sale ID {sale.Id}: IsActive={sale.IsActive}, StartsAt={sale.StartsAt}, EndsAt={sale.EndsAt}, Now={DateTime.UtcNow}");

                            if (sale.IsActive && sale.EndsAt < DateTime.UtcNow)
                            {
                                saleService.DeactivateSale(sale.Id);
                                _logger.LogInformation($"Deactivated sale with ID {sale.Id}");
                            }
                            else if (!sale.IsActive && sale.StartsAt <= DateTime.UtcNow && sale.EndsAt >= DateTime.UtcNow)
                            {
                                saleService.ActivateSaleWithDefaultDates(sale.Id);
                                _logger.LogInformation($"Activated sale with ID {sale.Id}");
                            }
                            else
                            {
                                _logger.LogInformation("nothing happenedddddddddddddddddddddddddddddddddddddddddddddddddddddddd");                          }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in background service of Ssale: {ex.Message}");
                }
                // Wait 1 minute before running again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}