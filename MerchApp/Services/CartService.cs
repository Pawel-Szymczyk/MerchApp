using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MerchApp.Services
{
    public class CartService : ICartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
        public int TotalQuantity => _items.Sum(i => i.Quantity);
        public bool IsEmpty => _items.Count == 0;

        public event EventHandler? CartChanged;

        public void AddItem(Item item, int quantity = 1)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

            var existing = FindLine(item);

            if (existing is not null)
            {
                //existing.Quantity = Math.Min(
                //    existing.Quantity + quantity,
                //    item.AvailableCount);
                existing.Quantity += quantity;
            }
            else
            {
                //_items.Add(new CartItem
                //{
                //    Item = item,
                //    Quantity = Math.Min(quantity, item.AvailableCount)
                //});
                _items.Add(new CartItem { Item = item, Quantity = quantity });
            }

            RaiseCartChanged();
        }

        public void RemoveItem(Item item, int quantity = 1)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

            var existing = FindLine(item);
            if (existing is null) return;

            existing.Quantity -= quantity;

            if (existing.Quantity <= 0)
                _items.Remove(existing);

            RaiseCartChanged();
        }

        public void SetQuantity(Item item, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            var existing = FindLine(item);

            if (quantity == 0)
            {
                if (existing is not null)
                {
                    _items.Remove(existing);
                    RaiseCartChanged();
                }
                return;
            }

            if (existing is not null)
                //existing.Quantity = Math.Min(quantity, item.AvailableCount);
                existing.Quantity = quantity;
            else
                _items.Add(new CartItem
                {
                    Item = item,
                    //Quantity = Math.Min(quantity, item.AvailableCount)
                    Quantity = quantity
                });

            RaiseCartChanged();
        }

        public bool Contains(Item item) => FindLine(item) is not null;

        public int GetQuantity(Item item) => FindLine(item)?.Quantity ?? 0;

        public void Clear()
        {
            _items.Clear();
            RaiseCartChanged();
        }

        private CartItem? FindLine(Item item) =>
            _items.FirstOrDefault(c => c.Item.Id == item.Id);

        private void RaiseCartChanged() =>
            CartChanged?.Invoke(this, EventArgs.Empty);
    }
}
