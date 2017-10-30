namespace Limbs.Web.Entities.Models
{
    /// <summary>
    /// Application authorization roles
    /// </summary>
    public class AppRoles
    {
        /// <summary>
        /// Administrator IS a member of Atomic
        /// </summary>
        public const string Administrator = "Administrator";
        /// <summary>
        /// User IS NOT a member of Atomic
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Registered user without a specific role
        /// </summary>
        public const string Unassigned = "Unassigned";
        /// <summary>
        /// Registered user who is requesting orders
        /// </summary>
        public const string Requester = "Requester";
        /// <summary>
        /// Registered user who is processing orders
        /// </summary>
        public const string Ambassador = "Ambassador";
    }
}