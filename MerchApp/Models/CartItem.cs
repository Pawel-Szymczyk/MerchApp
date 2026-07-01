using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    public class CartItem
    {
        public Item Item { get; set; } = null!;
        public string DisplayLine => Item.Title;
    }
}
