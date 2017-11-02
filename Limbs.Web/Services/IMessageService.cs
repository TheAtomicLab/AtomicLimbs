using System.Threading.Tasks;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Services
{
    public interface IMessageService
    {
        Task<int> Send(MessageModel message);
    }
}
