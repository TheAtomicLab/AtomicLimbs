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
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.Username"];
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
                        Subject = $"[Atomic Limbs] Aceptaste orden (orden {order.Id})",
                        To = order.OrderAmbassador.Email,
                        Body = "", //TODO (ale): segun template
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Solicitud de orden (orden {order.Id})",
                        To = order.OrderRequestor.Email,
                        Body = "", //TODO (ale): segun template
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);

                    break;
                case OrderStatus.Ready:

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Coordinar envío (orden {order.Id})",
                        To = _adminEmails,
                        Body = "", //TODO (ale): segun template
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Coordinar envío (orden {order.Id})",
                        To = order.OrderAmbassador.Email,
                        Body = "", //TODO (ale): segun template
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);

                    mailMessage = new MailMessage
                    {
                        From = _fromEmail,
                        Subject = $"[Atomic Limbs] Pedido listo (orden {order.Id})",
                        To = order.OrderAmbassador.Email,
                        Body = "", //TODO (ale): segun template
                    };
                    await AzureQueue.EnqueueAsync(mailMessage);
                    
                    break;
                case OrderStatus.Delivered:
                    //Entra por SendDeliveryInformationNotification
                    break;
            }
        }

        public async Task SendAmbassadorChangedNotification(OrderModel order, AmbassadorModel oldAmbassador, AmbassadorModel newAmbassador)
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Solicitud de orden (orden {order.Id})",
                To = newAmbassador.Email,
                Body = CompiledTemplateEngine.Render("Mails.Generic", order),
            };
            await AzureQueue.EnqueueAsync(mailMessage);

            if (oldAmbassador != null && oldAmbassador != newAmbassador) //tenia otro embajador
            {
                mailMessage = new MailMessage
                {
                    From = _fromEmail,
                    Subject = $"[Atomic Limbs] Cambio de embajador (orden {order.Id})",
                    To = oldAmbassador.Email,
                    Body = "" //TODO (ale): segun template
                };
                await AzureQueue.EnqueueAsync(mailMessage);
            }

        }

        public async Task SendDeliveryInformationNotification(OrderModel order) 
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Información de envío (orden {order.Id})",
                To = order.OrderAmbassador.Email,
                Cc = order.OrderRequestor.Email,
                Body = "" //TODO (ale): segun template
            };

            await AzureQueue.EnqueueAsync(mailMessage);
        }

        public async Task SendProofOfDeliveryNotification(OrderModel order)
        {
            var mailMessage = new MailMessage
            {
                From = _fromEmail,
                Subject = $"[Atomic Limbs] Nueva prueba de entrega (orden {order.Id})",
                To = order.OrderAmbassador.Email,
                Cc = order.OrderRequestor.Email,
                Body = "" //TODO (ale): segun template
            };

            await AzureQueue.EnqueueAsync(mailMessage);
        }
    }
}