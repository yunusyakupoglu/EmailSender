using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.ViewModels
{
    public class BaseViewModel
    {
        public bool IsOk { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
