using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// Represents a single merch item stored in the SharePoint 'MerchItems' list.
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
