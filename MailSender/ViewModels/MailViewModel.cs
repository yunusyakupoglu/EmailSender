using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.ViewModels
{
    public class MailViewModel : BaseViewModel
    {
        public MailViewModel()
        {
            MailAddresses = new List<string>();
        }
        public List<string> MailAddresses { get; set; }

       
    }
}
