using System;
using System.Collections.Generic;

namespace Limbs.Web.Areas.Admin.Models
{
    public class AppExceptionViewModel
    {
        public DateTime Date { get; set; }

        public List<AppExeptionItemViewModel> List { get; set; }
    }

    public class AppExeptionItemViewModel
    {
        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Url { get; set; }

        public string CustomMessage { get; set; }
    }
}