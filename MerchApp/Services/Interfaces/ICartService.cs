using MerchApp.Models;
using System;
using System.Collections.Generic;

namespace MerchApp.Services.Interfaces
{    public interface ICartService
    {
        IReadOnlyList<CartItem> Items { get; }
        int TotalCount { get; }
        bool IsEmpty { get; }

        event EventHandler CartChanged;

        void AddItem(Item item);
        void RemoveItem(Item item);
        bool Contains(Item item);
        void Clear();
    }
}