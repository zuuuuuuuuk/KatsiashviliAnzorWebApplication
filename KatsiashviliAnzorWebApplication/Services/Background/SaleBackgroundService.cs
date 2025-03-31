using KatsiashviliAnzorWebApplication.Services.Abstraction;
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

                        foreach (var sale in sales)
                        {
                            // Check if the sale is active and should be deactivated
                            if (sale.IsActive && sale.EndsAt < DateTime.UtcNow)
                            {
                                // Deactivate sale if it has ended
                                saleService.DeactivateSale(sale.Id);
                                _logger.LogInformation($"Deactivated sale with ID {sale.Id}");
                            }
                            // Check if the sale should be activated
                           if (!sale.IsActive && sale.StartsAt <= DateTime.UtcNow)
                            {
                                // Activate sale if it should start
                                saleService.ActivateSaleWithDefaultDates(sale.Id);
                                _logger.LogInformation($"Activated sale with ID {sale.Id}");
                            }
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