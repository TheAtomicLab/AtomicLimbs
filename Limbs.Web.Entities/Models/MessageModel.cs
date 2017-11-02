using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public DateTime Time { get; set; }
        
        public Priority Priority { get; set; }

        public ApplicationUser From { get; set; }

        public ApplicationUser To { get; set; }

        public string Content { get; set; }

        public MessageStatus Status { get; set; }

        public OrderModel Order { get; set; }

        public MessageModel PreviousMessage { get; set; }
    }

    public enum Priority
    {
        Low,
        Normal,
        High
    }

    public enum MessageStatus
    {
        Unread,
        Read,
        Archived,
        Deleted
    }
}