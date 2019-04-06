using System.Threading.Tasks;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Logic.Services
{
    public interface IOrderNotificationService
    {
        Task SendStatusChangeNotification(OrderModel order, OrderStatus oldStatus, OrderStatus newStatus);
        
        Task SendAmbassadorChangedNotification(OrderModel order, AmbassadorModel oldAmbassador, AmbassadorModel newAmbassador);

        Task SendDeliveryInformationNotification(OrderModel order);

        Task SendProofOfDeliveryNotification(OrderModel order);

        Task SendNewOrderNotificacion(OrderModel order);

    }
}
