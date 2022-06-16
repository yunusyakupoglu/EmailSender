using MailSender.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IExchangeService _exchangeService;
        private readonly IExcelService _excelService;
        private readonly IMailService _mailService;

        public Worker(ILogger<Worker> logger, IExchangeService exchangeService, IMailService mailService, IExcelService excelService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
            _mailService = mailService;
            _excelService = excelService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Consume Scoped Service Hosted Service running.");
                await DoWork(stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
             "Application started.");
                var liste = await _exchangeService.GetExchangeDataAsync();
                //TODO: liste kontrolu
                var mailListResponse = await _excelService.ReadExcelFile();
                if (!mailListResponse.IsOk)
                    Console.WriteLine(mailListResponse.ErrorMessage);
               
                var sendMailResponse = await _mailService.SendMail(liste,mailListResponse);
                    if (sendMailResponse)
                        await Task.Delay(TimeSpan.FromDays(1));  //TODO : Timespan 1 gün olacak
                    else
                        await Task.Delay(TimeSpan.FromMinutes(5)); // 5 dakika sonra tekrar dene
              
                   
            }

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
