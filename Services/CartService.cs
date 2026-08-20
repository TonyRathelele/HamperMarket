using System.Text.Json;
using HamperMarket.Models;
using Microsoft.AspNetCore.Http;

namespace HamperMarket.Services
{
    public class CartService
    {
        private const string SessionKey = "HamperMarket_Cart";
        private readonly IHttpContextAccessor _accessor;

        public CartService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private ISession Session => _accessor.HttpContext!.Session;

        public List<CartItem> GetItems()
        {
            var json = Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveItems(List<CartItem> items)
        {
            Session.SetString(SessionKey, JsonSerializer.Serialize(items));
        }

        public void AddItem(string productId, int quantity)
        {
            var items = GetItems();
            var existing = items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }
            SaveItems(items);
        }

        public void UpdateQuantity(string productId, int quantity)
        {
            var items = GetItems();
            var existing = items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                if (quantity <= 0)
                    items.Remove(existing);
                else
                    existing.Quantity = quantity;
            }
            SaveItems(items);
        }

        public void RemoveItem(string productId)
        {
            var items = GetItems();
            items.RemoveAll(i => i.ProductId == productId);
            SaveItems(items);
        }

        public void Clear()
        {
            Session.Remove(SessionKey);
        }

        public int TotalCount() => GetItems().Sum(i => i.Quantity);
    }
}
