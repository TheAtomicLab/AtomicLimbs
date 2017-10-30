using Limbs.Web.Models;

namespace Limbs.Web.Services
{
    public interface IMessageService
    {
        void Send(MessageModel message);
    }
}
