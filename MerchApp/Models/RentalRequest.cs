using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// A rental request submitted by a user.
    /// Stored in SharePoint 'RentalRequests' list.
    /// One request can contain multiple items (see RentalItem).
    /// </summary>
    public class RentalRequest
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime RentalFrom { get; set; }
        public DateTime RentalTo { get; set; }
        public DateTime? ReturnedDate { get; set; }

        public RentalStatus Status { get; set; } = RentalStatus.Pending;

        public string Purpose { get; set; } = string.Empty;
        public string ManagerNote { get; set; } = string.Empty;

        public List<RentalItem> Items { get; set; } = new();

        public string StatusDisplayName => Status switch
        {
            RentalStatus.Pending => "Pending",
            RentalStatus.Approved => "Approved",
            RentalStatus.Rejected => "Rejected",
            RentalStatus.Returned => "Returned",
            _ => "Unknown"
        };

        public bool IsOverdue =>
            Status == RentalStatus.Approved &&
            RentalTo < DateTime.Today &&
            ReturnedDate == null;
    }
}
