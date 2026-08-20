using HamperMarket.Models;
using HamperMarket.Services;

namespace HamperMarket.Data
{
    public static class DataSeeder
    {
        public static void Seed(JsonStore<User> users, JsonStore<Product> products)
        {
            if (!users.GetAll().Any(u => u.Role == UserRole.Admin))
            {
                var (hash, salt) = PasswordHasher.Hash("Admin@123");
                users.Add(new User
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

            if (users.GetAll().Any(u => u.Role == UserRole.Seller)) return; // already seeded

            var (h1, s1) = PasswordHasher.Hash("Seller@123");
            var seller1 = new User
            {
                Role = UserRole.Seller,
                DisplayName = "Karoo Gift Co.",
                Email = "seller@hampermarket.demo",
                Phone = "082 555 0101",
                Bio = "Handmade Karoo-style gourmet hampers, biltong boxes and biscuits.",
                LogoEmoji = "🧺",
                PasswordHash = h1,
                PasswordSalt = s1,
                Status = SellerStatus.Approved
            };

            var (h2, s2) = PasswordHasher.Hash("Seller@123");
            var seller2 = new User
            {
                Role = UserRole.Seller,
                DisplayName = "Bloom & Basket",
                Email = "bloom@hampermarket.demo",
                Phone = "082 555 0202",
                Bio = "Spa, wellness and flower-themed gift hampers for every occasion.",
                LogoEmoji = "🌸",
                PasswordHash = h2,
                PasswordSalt = s2,
                Status = SellerStatus.Approved
            };

            var (h3, s3) = PasswordHasher.Hash("Seller@123");
            var seller3 = new User
            {
                Role = UserRole.Seller,
                DisplayName = "New Seller Co.",
                Email = "pending@hampermarket.demo",
                Phone = "082 555 0303",
                Bio = "A brand new seller awaiting admin approval (demo of the approval flow).",
                LogoEmoji = "🆕",
                PasswordHash = h3,
                PasswordSalt = s3,
                Status = SellerStatus.PendingApproval
            };

            users.AddRange(new[] { seller1, seller2, seller3 });

            var demoProducts = new List<Product>
            {
                new Product
                {
                    SellerId = seller1.Id, Name = "Karoo Biltong & Droëwors Box", Category = "Snack Hampers",
                    Description = "A generous selection of grass-fed beef biltong, droëwors and chili bites, packed in a rustic wooden crate.",
                    Price = 449.00m, Stock = 25, ImageEmoji = "🥩", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Best Seller", "Gift for Him", "Braai" }
                },
                new Product
                {
                    SellerId = seller1.Id, Name = "Farmstyle Breakfast Hamper", Category = "Food Hampers",
                    Description = "Local jams, rusks, honey and freshly roasted coffee beans for a lazy Sunday morning.",
                    Price = 389.50m, Stock = 18, ImageEmoji = "🍯", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Breakfast", "Family" }
                },
                new Product
                {
                    SellerId = seller1.Id, Name = "Ultimate Braai Master Crate", Category = "Snack Hampers",
                    Description = "Everything for the ultimate braai: spice rubs, sauces, tongs and boerewors seasoning.",
                    Price = 599.00m, Stock = 12, ImageEmoji = "🔥", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Gift for Him", "Braai", "Premium" }
                },
                new Product
                {
                    SellerId = seller2.Id, Name = "Rose & Relax Spa Hamper", Category = "Spa & Wellness",
                    Description = "Rose-scented bath salts, a soy candle, body butter and a soft towel wrap.",
                    Price = 529.00m, Stock = 20, ImageEmoji = "🌹", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Gift for Her", "Spa", "Best Seller" }
                },
                new Product
                {
                    SellerId = seller2.Id, Name = "New Baby Bloom Box", Category = "Baby & New Mom",
                    Description = "Soft blankets, baby-safe skincare and a handwritten congratulations card.",
                    Price = 459.00m, Stock = 15, ImageEmoji = "🍼", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Baby", "New Mom" }
                },
                new Product
                {
                    SellerId = seller2.Id, Name = "Self-Care Sunday Box", Category = "Spa & Wellness",
                    Description = "Face masks, herbal tea, a journal and calming lavender oil roller.",
                    Price = 349.00m, Stock = 30, ImageEmoji = "🧖", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Gift for Her", "Wellness" }
                },
                new Product
                {
                    SellerId = seller2.Id, Name = "Festive Christmas Cheer Hamper", Category = "Seasonal",
                    Description = "Mulled wine spices, shortbread, mince pies and festive decor for the holidays.",
                    Price = 619.00m, Stock = 10, ImageEmoji = "🎄", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Christmas", "Family", "Premium" }
                },
                new Product
                {
                    SellerId = seller1.Id, Name = "Vegan Delights Snack Box", Category = "Food Hampers",
                    Description = "A plant-based selection of nut butters, dried fruit, trail mix and vegan chocolate.",
                    Price = 379.00m, Stock = 22, ImageEmoji = "🥑", Status = ProductStatus.Approved,
                    Labels = new List<string> { "Vegan", "Healthy" }
                }
            };

            products.AddRange(demoProducts);
        }
    }
}
