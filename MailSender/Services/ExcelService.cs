using MailSender.ViewModels;
using Microsoft.Extensions.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Services
{
    public class ExcelService : IExcelService
    {
        private IHostingEnvironment _env;
        private IWorkbook _workbook;
        private ISheet _sheet;
        private IRow _currentRow;
        private readonly IExceptionService _exceptionService;


        public ExcelService(IHostingEnvironment env, IExceptionService exceptionService)
        {
            _env = env;
            _exceptionService = exceptionService;
        }

        public Task<MailViewModel> ReadExcelFile()
        {
            var pathToFile = new StringBuilder(_env.ContentRootPath + "\\MailData" + "\\mymails.xlsx").ToString();

            try
            {
                MailViewModel mails = new MailViewModel();
                FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);

                if (pathToFile.IndexOf(".xlsx") > 0)
                {
                    _workbook = null;
                    _workbook = new XSSFWorkbook(fs);
                    _sheet = _workbook.GetSheetAt(0);
                    if (_sheet != null)
                    {
                        int rowCount = _sheet.LastRowNum;
                        for (int i = 0; i <= rowCount; i++)
                        {
                            _currentRow = _sheet.GetRow(i);
                            var cellValue = _currentRow.GetCell(0).StringCellValue.Trim();
                            mails.MailAddresses.Add(cellValue);
                            mails.IsOk = true;
                        }
                        return Task.FromResult(mails);
                    }
                    mails.ErrorMessage = "Sheet Bulunamadı";
                    return Task.FromResult(mails);
                }
                mails.ErrorMessage = "Excel dosyası Bulunamadı";
                return Task.FromResult(mails);

            }
            catch (Exception ex)
            {
                _exceptionService.WriteErrorFile(ex);
                throw;
            }

        }
    }
}
