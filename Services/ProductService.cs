using HamperMarket.Data;
using HamperMarket.Models;

namespace HamperMarket.Services
{
    public class ProductService
    {
        private readonly JsonStore<Product> _store;

        public ProductService(JsonStore<Product> store)
        {
            _store = store;
        }

        public List<Product> GetAll() => _store.GetAll();

        public List<Product> GetApproved() =>
            _store.GetAll().Where(p => p.Status == ProductStatus.Approved).OrderByDescending(p => p.CreatedAt).ToList();

        public List<Product> Search(string? query, string? category, string? label)
        {
            var items = GetApproved();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                items = items.Where(p =>
                    p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Labels.Any(l => l.Contains(q, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                items = items.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(label))
            {
                items = items.Where(p => p.Labels.Any(l => l.Equals(label, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            return items;
        }

        public List<string> GetCategories() =>
            GetApproved().Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

        public List<string> GetLabels() =>
            GetApproved().SelectMany(p => p.Labels).Distinct().OrderBy(l => l).ToList();

        public Product? GetById(string id) => _store.GetAll().FirstOrDefault(p => p.Id == id);

        public List<Product> GetBySeller(string sellerId) =>
            _store.GetAll().Where(p => p.SellerId == sellerId).OrderByDescending(p => p.CreatedAt).ToList();

        public Product Create(Product product)
        {
            _store.Add(product);
            return product;
        }

        public void Update(string id, Action<Product> updateAction)
        {
            _store.Update(p => p.Id == id, p =>
            {
                updateAction(p);
                p.UpdatedAt = DateTime.UtcNow;
            });
        }

        public void Delete(string id)
        {
            _store.Remove(p => p.Id == id);
        }

        public void SetStatus(string id, ProductStatus status)
        {
            Update(id, p => p.Status = status);
        }

        public void DecrementStock(string id, int qty)
        {
            _store.Update(p => p.Id == id, p => p.Stock = Math.Max(0, p.Stock - qty));
        }
    }
}
