using MerchApp.Models;
using MerchApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MerchApp.Services
{
    public class CartService : ICartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
        public int TotalCount => _items.Count;
        public bool IsEmpty => _items.Count == 0;

        public event EventHandler? CartChanged;

        public void AddItem(Item item)
        {
            if (Contains(item)) return;
            _items.Add(new CartItem { Item = item });
            RaiseCartChanged();
        }

        public void RemoveItem(Item item)
        {
            var existing = _items.FirstOrDefault(c => c.Item.Id == item.Id);
            if (existing is null) return;
            _items.Remove(existing);
            RaiseCartChanged();
        }

        public bool Contains(Item item) =>
            _items.Any(c => c.Item.Id == item.Id);

        public void Clear()
        {
            _items.Clear();
            RaiseCartChanged();
        }

        private void RaiseCartChanged() =>
            CartChanged?.Invoke(this, EventArgs.Empty);
    }
}
