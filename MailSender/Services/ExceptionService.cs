using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly IHostEnvironment _env;

        public ExceptionService(IHostEnvironment env)
        {
            _env = env;
        }

        public void WriteErrorFile(Exception ex)
        {
            string pathExtention = ".txt";
            var pathToExceptionFile = new StringBuilder("C:\\EmailSender" + "\\Exceptions\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "-"
                + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + "Exception" + pathExtention).ToString();
            if (!File.Exists(pathToExceptionFile))
            {
                File.Create(pathToExceptionFile).Dispose();
            }
            using (StreamWriter writer = new StreamWriter(pathToExceptionFile, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
    }
}
