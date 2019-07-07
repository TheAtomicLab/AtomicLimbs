using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Limbs.Web.Entities.Resources;

namespace Limbs.Web.Entities.Models
{
    /// <summary>
    /// Message entity from internal messaging. 1:1 messaging
    /// </summary>
    public class MessageModel
    {
        public MessageModel()
        {
            Status = MessageStatus.Unread;
            Priority = Priority.Normal;
            Time = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name = "Message_Time", Description = "", ResourceType = typeof(ModelTexts))]
        public DateTime Time { get; set; }
        
        public Priority Priority { get; set; }

        [Display(Name = "Message_From", Description = "", ResourceType = typeof(ModelTexts))]
        public ApplicationUser From { get; set; }

        [Display(Name = "Message_To", Description = "", ResourceType = typeof(ModelTexts))]
        public ApplicationUser To { get; set; }

        public string Content { get; set; }

        public MessageStatus Status { get; set; }

        public OrderModel Order { get; set; }

        public MessageModel PreviousMessage { get; set; }
    }

    public enum Priority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public enum MessageStatus
    {
        Unread = 0,
        Read = 1,
        Archived = 2,
        Deleted = 3
    }
}