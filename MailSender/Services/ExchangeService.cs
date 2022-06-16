using MailSender.ViewModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MailSender.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExceptionService _exceptionService;

        public ExchangeService(IExceptionService exceptionService)
        {
            _exceptionService = exceptionService;
        }

        public Task<ExchangeViewModel> GetExchangeDataAsync()
        {
            try
            {
                ExchangeViewModel exchangeViewModel = new ExchangeViewModel();
                XmlDocument xmlData = new XmlDocument();
                xmlData.Load("http://www.tcmb.gov.tr/kurlar/today.xml");

                exchangeViewModel.DollarSellingPrice = xmlData.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "USD")).InnerText.Replace('.', ',');
                exchangeViewModel.EuroSellingPrice = xmlData.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "EUR")).InnerText.Replace('.', ',');

                exchangeViewModel.DollarBuyingPrice = xmlData.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexBuying", "USD")).InnerText.Replace('.', ',');
                exchangeViewModel.EuroBuyingPrice = xmlData.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexBuying", "EUR")).InnerText.Replace('.', ',');
                exchangeViewModel.ExchangeRate = (decimal.Parse(exchangeViewModel.DollarBuyingPrice) / decimal.Parse(exchangeViewModel.EuroBuyingPrice)).ToString();
                return Task.FromResult(exchangeViewModel);
            }
            catch (Exception ex)
            {
                _exceptionService.WriteErrorFile(ex);
                throw;
            }

        }


    }
}
