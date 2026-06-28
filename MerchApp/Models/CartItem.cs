using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// Transient — lives only in memory while the user is building their request.
    /// Not stored in SharePoint until the request is submitted.
    /// </summary>
    public class CartItem
    {
        public Item Item { get; set; } = null!;
        public int Quantity { get; set; } = 1;

        public string DisplayLine => $"{Item.Title}  ×{Quantity}";
    }
}
