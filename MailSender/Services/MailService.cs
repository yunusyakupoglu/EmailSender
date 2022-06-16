using MailKit.Net.Smtp;
using MailKit.Security;
using MailSender.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Services
{
    public class MailService : IMailService
    {
        private IHostingEnvironment _env;
        private readonly IExceptionService _exceptionService;
        private IConfiguration Configuration;

        public MailService(IHostingEnvironment env, IExceptionService exceptionService, IConfiguration configuration)
        {
            _env = env;
            _exceptionService = exceptionService;
            Configuration = configuration;
        }

        public Task<bool> SendMail(ExchangeViewModel getExchangeData, MailViewModel mails)
        {
            try
            {
                var CurrentDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm"); // TODO: DateTime.Now.ToShortDate ' de olabilir duruma bağlı
                var pathToFile = new StringBuilder(_env.ContentRootPath + "\\Template" + "\\MailTemplate.html").ToString();
                var builder = new BodyBuilder();

                using (StreamReader SourceReader = File.OpenText(pathToFile))
                {

                    builder.HtmlBody = SourceReader.ReadToEnd();

                }
                builder.HtmlBody = builder.HtmlBody.Replace("{Gun}", CurrentDate).Replace("{DollarBuyingPrice}", getExchangeData.DollarBuyingPrice).Replace("{DollarSellingPrice}", getExchangeData.DollarSellingPrice)
                    .Replace("{EuroBuyingPrice}", getExchangeData.EuroBuyingPrice).Replace("{EuroSellingPrice}", getExchangeData.EuroSellingPrice).Replace("{ExchangeRate}", getExchangeData.ExchangeRate);


                string host = this.Configuration.GetValue<string>("Smtp:Server");
                int port = this.Configuration.GetValue<int>("Smtp:Port");
                string fromAddress = this.Configuration.GetValue<string>("Smtp:FromAddress");
                string userName = this.Configuration.GetValue<string>("Smtp:UserName");
                string password = this.Configuration.GetValue<string>("Smtp:Password");

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromAddress));
                foreach (var item in mails.MailAddresses)
                {
                    email.To.Add(MailboxAddress.Parse(item));
                }

                email.Subject = "Günlük Döviz Kuru";
                email.Body = new TextPart(TextFormat.Html) { Text = builder.HtmlBody };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(host, port, SecureSocketOptions.StartTls);
                //TODO: validations authenticate oldu mu, gönderildi mi
                smtp.Authenticate(userName, password);
                smtp.Send(email);
                smtp.Disconnect(true);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _exceptionService.WriteErrorFile(ex);
                return Task.FromResult(false);
            }
        }
    }
}
