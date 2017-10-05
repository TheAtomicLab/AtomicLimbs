using System.Threading.Tasks;
using Limbs.Web.Models;

namespace Limbs.Web.Services
{
    public class OrderMailNotificationService : IOrderNotificationService
    {
        public async Task SendStatusChangeNotification(OrderModel order, OrderStatus oldStatus, OrderStatus newStatus)
        {
            //TODO (ale): implement
            return;
        }
        
        public async Task SendAmbassadorChangedNotification(OrderModel order, AmbassadorModel oldAmbassador, AmbassadorModel newAmbassador)
        {
            //TODO (ale): implement
            return;
        }

        public async Task SendDeliveryInformationNotification(OrderModel order)
        {
            //TODO (ale): implement
            return;

        }

        public async Task SendProofOfDeliveryNotification(OrderModel order)
        {
            //TODO (ale): implement
            return;

        }
    }
}