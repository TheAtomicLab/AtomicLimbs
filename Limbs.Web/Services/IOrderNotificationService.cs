using System.Threading.Tasks;
using Limbs.Web.Models;

namespace Limbs.Web.Services
{
    public interface IOrderNotificationService
    {
        Task SendStatusChangeNotification(OrderModel order, OrderStatus oldStatus, OrderStatus newStatus);
        
        Task SendAmbassadorChangedNotification(OrderModel order, AmbassadorModel oldAmbassador, AmbassadorModel newAmbassador);

        Task SendDeliveryInformationNotification(OrderModel order);

        Task SendProofOfDeliveryNotification(OrderModel order);

    }
}
