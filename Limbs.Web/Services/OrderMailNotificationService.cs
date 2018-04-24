using System.Configuration;
using System.Threading.Tasks;
using Limbs.Web.Common.Mail;
using Limbs.Web.Entities.Models;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;

namespace Limbs.Web.Services
{
    public class OrderMailNotificationService : IOrderNotificationService
    {
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];
        private readonly string _adminEmails = ConfigurationManager.AppSettings["AdminEmails"];

        public async Task SendStatusChangeNotification(OrderModel order, OrderStatus oldStatus, OrderStatus newStatus)
        {
            if (oldStatus == newStatus) return;

            switch (newStatus)
            {
                case OrderStatus.NotAssigned:
                    //No entra nunca
                    break;
                case OrderStatus.PreAssigned:
                    //Entra por SendAmbassadorChangedNotification
                    break;
                case OrderStatus.Pending:

                    var mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Aceptaste el pedido (#{order.Id})",
                        To = order.OrderAmbassador.Email,
                        Body = CompiledTemplateEngine.Render("Mails.OrderAcceptedToAmbassador", order),
                    };
                    if (!string.IsNullOrEmpty(order.OrderAmbassador.AlternativeEmail))
                        mailMessage.Cc = order.OrderAmbassador.AlternativeEmail;

                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Pedido aceptado por embajador (#{order.Id})",
                        To = order.OrderRequestor.Email,
                        Body = CompiledTemplateEngine.Render("Mails.OrderAcceptedToRequestor", order),
                    };
                    if (!string.IsNullOrEmpty(order.OrderRequestor.AlternativeEmail))
                        mailMessage.Cc = order.OrderRequestor.AlternativeEmail;

                    await AzureQueue.EnqueueAsync(mailMessage);

                    break;
                case OrderStatus.Ready:

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Coordinar envío (#{order.Id})",
                        To = _adminEmails,
                        Body = CompiledTemplateEngine.Render("Mails.OrderReadyToAdmin", order),
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Coordinar envío (#{order.Id})",
                        To = order.OrderAmbassador.Email,
                        Body = CompiledTemplateEngine.Render("Mails.OrderReadyToAmbassador", order),
                    };
                    if (!string.IsNullOrEmpty(order.OrderAmbassador.AlternativeEmail))
                        mailMessage.Cc = order.OrderAmbassador.AlternativeEmail;

                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Pedido listo (#{order.Id})",
                        To = order.OrderRequestor.Email,
                        Body = CompiledTemplateEngine.Render("Mails.OrderReadyToRequestor", order),
                    };
                    if (!string.IsNullOrEmpty(order.OrderRequestor.AlternativeEmail))
                        mailMessage.Cc = order.OrderRequestor.AlternativeEmail;

                    await AzureQueue.EnqueueAsync(mailMessage);
                    
                    break;
                case OrderStatus.Delivered:
                    //Entra por SendDeliveryInformationNotification
                    break;
                case OrderStatus.ArrangeDelivery:
                    //No action
                    break;
            }
        }

        public async Task SendAmbassadorChangedNotification(OrderModel order, AmbassadorModel oldAmbassador, AmbassadorModel newAmbassador)
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Nuevo pedido (#{order.Id})",
                To = newAmbassador.Email,
                Body = CompiledTemplateEngine.Render("Mails.OrderNewAmbassador", order),
            };
            if (!string.IsNullOrEmpty(newAmbassador.AlternativeEmail))
                mailMessage.Cc = newAmbassador.AlternativeEmail;
            
            await AzureQueue.EnqueueAsync(mailMessage);

            if (oldAmbassador != null && oldAmbassador != newAmbassador) //tenia otro embajador
            {
                order.OrderAmbassador = oldAmbassador;

                mailMessage = new MailMessage
                {
                    From = _fromEmail,
                    Subject = $"[Atomic Limbs] Cambio de embajador (pedido #{order.Id})",
                    To = oldAmbassador.Email,
                    Body = CompiledTemplateEngine.Render("Mails.OrderNewAmbassadorToOldAmbassador", order),
                };
                if (!string.IsNullOrEmpty(oldAmbassador.AlternativeEmail))
                    mailMessage.Cc = oldAmbassador.AlternativeEmail;

                await AzureQueue.EnqueueAsync(mailMessage);

                order.OrderAmbassador = newAmbassador;
            }

        }

        public async Task SendDeliveryInformationNotification(OrderModel order) 
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Información de envío (pedido #{order.Id})",
                To = order.OrderAmbassador.Email,
                Cc = order.OrderRequestor.Email,
                Body = CompiledTemplateEngine.Render("Mails.OrderDeliveryInformation", order),
            };

            await AzureQueue.EnqueueAsync(mailMessage);
        }

        public async Task SendProofOfDeliveryNotification(OrderModel order)
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Nueva prueba de entrega (pedido #{order.Id})",
                To = order.OrderRequestor.Email,
                Body = CompiledTemplateEngine.Render("Mails.OrderProofOfDeliveryInfoToRequestor", order),
            };
            if (!string.IsNullOrEmpty(order.OrderRequestor.AlternativeEmail))
                mailMessage.Cc = order.OrderRequestor.AlternativeEmail;

            await AzureQueue.EnqueueAsync(mailMessage);

            mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Nueva prueba de entrega (pedido #{order.Id})",
                To = order.OrderAmbassador.Email,
                Body = CompiledTemplateEngine.Render("Mails.OrderProofOfDeliveryInfoToAmbassador", order),
            };
            if (!string.IsNullOrEmpty(order.OrderAmbassador.AlternativeEmail))
                mailMessage.Cc = order.OrderAmbassador.AlternativeEmail;


            await AzureQueue.EnqueueAsync(mailMessage);
        }
    }
}