using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// Represents a single merch item stored in the SharePoint 'MerchItems' list.
    /// TotalCount = how many physically exist.
    /// AvailableCount = calculated dynamically (TotalCount minus active rentals).
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //public int TotalCount { get; set; } = 1;
        //public int AvailableCount { get; set; } = 1;
        //public bool IsAvailable => AvailableCount > 0;
    }
}
