using System.Data;
using System.IO;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Common.Mail
{
    public static class MailTemplatesRegistration
    {
        public static void Initialize()
        {
            CompiledTemplateEngine.Add<OrderModel>("Mails.Generic", GetStringTemplate("Limbs.Web.Common.Mail.Templates.Generic.cshtml"));
        }


        private static string GetStringTemplate(string templateFilePath)
        {
            var assembly = typeof(MailTemplatesRegistration).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(templateFilePath))
            {
                if (stream == null)
                    throw new NoNullAllowedException("No se pudo encontrar el Recurso " + templateFilePath);

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
