using MailSender.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Services
{
    public interface IExchangeService
    {
        Task<ExchangeViewModel> GetExchangeDataAsync();



    }
}
