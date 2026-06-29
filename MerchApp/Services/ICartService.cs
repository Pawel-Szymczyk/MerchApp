using MerchApp.Models;
using System;
using System.Collections.Generic;

namespace MerchApp.Services
{
    //public interface ICartService
    //{
    //    bool IsEmpty { get; }
    //    IReadOnlyList<CartItem> Items { get; }
    //    int TotalQuantity { get; }

    //    event EventHandler? CartChanged;

    //    void AddItem(Item item, int quantity = 1);
    //    void Clear();
    //    bool Contains(Item item);
    //    int GetQuantity(Item item);
    //    void RemoveItem(Item item, int quantity = 1);
    //    void SetQuantity(Item item, int quantity);
    //}

    public interface ICartService
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