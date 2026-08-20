using HamperMarket.Data;
using HamperMarket.Models;

namespace HamperMarket.Services
{
    public class UserService
    {
        private readonly JsonStore<User> _store;

        public UserService(JsonStore<User> store)
        {
            _store = store;
        }

        public List<User> GetAllSellers() =>
            _store.GetAll().Where(u => u.Role == UserRole.Seller).OrderByDescending(u => u.CreatedAt).ToList();

        public User? GetById(string id) => _store.GetAll().FirstOrDefault(u => u.Id == id);

        public User? GetByEmail(string email) =>
            _store.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public bool EmailExists(string email) => GetByEmail(email) != null;

        public User RegisterSeller(string businessName, string email, string password, string phone, string? bio, string logoEmoji)
        {
            var (hash, salt) = PasswordHasher.Hash(password);
            var user = new User
            {
                Role = UserRole.Seller,
                DisplayName = businessName,
                Email = email.Trim(),
                Phone = phone,
                Bio = bio,
                LogoEmoji = string.IsNullOrWhiteSpace(logoEmoji) ? "🎁" : logoEmoji,
                PasswordHash = hash,
                PasswordSalt = salt,
                Status = SellerStatus.PendingApproval
            };
            _store.Add(user);
            return user;
        }

        public User? ValidateLogin(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null) return null;
            return PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt) ? user : null;
        }

        public void SetSellerStatus(string sellerId, SellerStatus status)
        {
            _store.Update(u => u.Id == sellerId, u => u.Status = status);
        }

        public void SeedAdminIfMissing()
        {
            if (!_store.GetAll().Any(u => u.Role == UserRole.Admin))
            {
                var (hash, salt) = PasswordHasher.Hash("Admin@123");
                _store.Add(new User
                {
                    Role = UserRole.Admin,
                    DisplayName = "Marketplace Admin",
                    Email = "admin@hampermarket.demo",
                    Phone = "0800 000 000",
                    LogoEmoji = "🛠️",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Status = SellerStatus.Approved
                });
            }
        }
    }
}
