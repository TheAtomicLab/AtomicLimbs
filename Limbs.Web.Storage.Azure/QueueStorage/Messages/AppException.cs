using System;

namespace Limbs.Web.Storage.Azure.QueueStorage.Messages
{
    public class AppException
    {
        public Exception Exception { get; set; }
        public string CustomMessage { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string LogMessage { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerExceptionMessage { get; set; }
    }
}
