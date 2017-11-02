using System.Threading.Tasks;
using Limbs.Web.Entities.Models;
using Limbs.Web.Models;

namespace Limbs.Web.Services
{
    public class MessagesService : IMessageService
    {
        public ApplicationDbContext Db = new ApplicationDbContext();
        
        public async Task<int> Send(MessageModel message)
        {
            Db.Messages.Add(message);

            return await Db.SaveChangesAsync();
        }
    }
}