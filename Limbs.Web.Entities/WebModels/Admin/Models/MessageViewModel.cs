using System;
using System.ComponentModel.DataAnnotations;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels.Extensions;

namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class MessageViewModel
    {
        public MessageViewModel()
        {
            Status = MessageStatus.Unread;
            Priority = Priority.Normal;
            Time = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public Priority Priority { get; set; }

        [Required]
        public ApplicationUserViewModel From { get; set; }

        [Required]
        public ApplicationUserViewModel To { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public MessageStatus Status { get; set; }

        public OrderModel Order { get; set; }

        public MessageModel PreviousMessage { get; set; }

        public void AddFrom(ApplicationUser fromUser)
        {
            From = fromUser.ToViewModel();
        }

        public void AddTo(ApplicationUser toUser)
        {
            To = toUser.ToViewModel();
        }
    }

    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public bool Equals(ApplicationUser user)
        {
            return Id == user.Id;
        }
    }
}