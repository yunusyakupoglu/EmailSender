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
                _logger.LogInformation("Application started.");

                var sendMailResponse = false;
                bool isTimeCorrect = false;
                var SuccessfulDate = TimeSpan.FromMinutes(10);
                var RetryDate = TimeSpan.FromSeconds(5);

                // Check if datetime is 7am
                if (DateTime.Now.Hour == 7)
                    isTimeCorrect = true;
                if (!isTimeCorrect)
                {
                    await Task.Delay(SuccessfulDate);
                    break;
                }
                var datalist = await _exchangeService.GetExchangeDataAsync();
                //TODO: liste kontrolu
                var mailListResponse = await _excelService.ReadExcelFile();
                if (!mailListResponse.IsOk)
                    Console.WriteLine(mailListResponse.ErrorMessage);


                
                if(isTimeCorrect)
                     sendMailResponse = await _mailService.SendMail(datalist, mailListResponse);

                if (sendMailResponse)
                {
                    _logger.LogInformation("Posta gönderildi");
                    await Task.Delay(SuccessfulDate);  //TODO : Timespan 10 dk olacak
                }
                else
                {
                    await Task.Delay(RetryDate); // baþarýsýzsa 25 saniye sonra tekrar dene 
                    _logger.LogInformation("Posta gönderilemedi, 25 saniye sonra tekrar denenecek");
                }


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
