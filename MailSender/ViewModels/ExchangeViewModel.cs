using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.ViewModels
{
    public class ExchangeViewModel : BaseViewModel
    {
        public string DollarSellingPrice { get; set; }
        public string DollarBuyingPrice { get; set; }
        public string EuroSellingPrice { get; set; }
        public string EuroBuyingPrice { get; set; }
        public string ExchangeRate { get; set; }
    }
}
