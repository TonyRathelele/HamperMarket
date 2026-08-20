using HamperMarket.Data;
using HamperMarket.Models;

namespace HamperMarket.Services
{
    public class OrderService
    {
        private readonly JsonStore<Order> _store;

        public OrderService(JsonStore<Order> store)
        {
            _store = store;
        }

        public List<Order> GetAll() => _store.GetAll().OrderByDescending(o => o.CreatedAt).ToList();

        public Order? GetById(string id) => _store.GetAll().FirstOrDefault(o => o.Id == id);

        public List<Order> GetForSeller(string sellerId) =>
            GetAll().Where(o => o.Items.Any(i => i.SellerId == sellerId)).ToList();

        public List<Order> GetForBuyer(string email) =>
            GetAll().Where(o => o.BuyerEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();

        public Order Create(Order order)
        {
            _store.Add(order);
            return order;
        }

        public void Update(string id, Action<Order> updateAction)
        {
            _store.Update(o => o.Id == id, updateAction);
        }

        public void SetStatus(string id, OrderStatus status)
        {
            Update(id, o => o.Status = status);
        }
    }
}
