# HamperMarket

**Richfield College of Southern Africa**
**Semester 2 — ASP.NET Development**

## Project Overview

HamperMarket is a multi-seller marketplace platform built for the ASP.NET module,
demonstrating a full-stack web application using ASP.NET Core MVC (C#) with Razor
Views. The platform allows independent sellers to register, list, and label their
own gift hamper products, while an admin moderates the marketplace. Buyers can
browse, add items to a cart, and complete a simulated checkout including South
African PEP/PAXI shipping options and a demo payment gateway.

No external database engine or SQLite is used. All application data (users,
products, orders, and simulated notifications) is persisted to JSON files under
`App_Data/`, implemented as a lightweight custom data-access layer.

## Features

- **Landing page** showcasing featured hampers and categories
- **Public shop** with search, category, and label filtering
- **Seller registration & login** (custom cookie-based authentication, no
  ASP.NET Identity)
- **Seller dashboard** — create, edit, label, and delete hamper listings
- **Admin panel** — approve/suspend sellers, approve/reject products, view all
  orders, and review a simulated email/SMS notification log
- **Shopping cart** (session-based, guest checkout supported)
- **Checkout flow** with simulated **PEP/PAXI** shipping options (collection
  point selection or door-to-door courier)
- **Simulated payment gateway** (demo card approval/decline logic)
- **Simulated email & SMS notifications** for order confirmations and seller
  updates

## Technology Stack

- ASP.NET Core 8 (MVC, Razor Views)
- C#
- Bootstrap 5 (vendored locally)
- Custom JSON file-based data store (no database/SQLite)
- Cookie authentication & session-based cart

## Project Structure


## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## How to Run

```bash
cd HamperMarket
dotnet restore
dotnet run
```

Open the URL shown in the console (e.g. `http://localhost:5000`).

On first run, the application seeds demo data automatically, including an admin
account, two approved sellers with sample hamper products, and one seller
account pending approval (to demonstrate the moderation workflow).

## Demo Accounts

| Role                | Email                       | Password    |
|---------------------|------------------------------|-------------|
| Admin                | admin@hampermarket.demo      | Admin@123   |
| Seller (approved)    | seller@hampermarket.demo     | Seller@123  |
| Seller (approved)    | bloom@hampermarket.demo      | Seller@123  |
| Seller (pending)     | pending@hampermarket.demo    | Seller@123  |

Buyers do not require an account — checkout is guest-only.

## Suggested Demo Walkthrough

1. Browse the shop as a guest, add a hamper to the cart, and proceed to checkout.
2. Select **PAXI Standard/Express** to see the collection-point picker, or
   **PEP Door-to-Door** to enter a delivery address.
3. Complete payment using any card number (e.g. `4242 4242 4242 4242`), or use
   `4000 0000 0000 0002` to simulate a declined payment.
4. Log in as admin and open **Simulated Messages** to view the generated
   confirmation email/SMS for the order.
5. Log in as `pending@hampermarket.demo` to see login blocked pending approval.
   Approve the account as admin, then log back in as that seller.
6. As a seller, add a new hamper with labels (e.g. `Vegan, Gift for Her`) and
   note that it only appears in the public shop once approved by an admin.

## Academic Note

This project was developed as part of the ASP.NET module coursework at
Richfield College of Southern Africa, Semester 2. All payment, shipping, and
notification functionality is simulated for demonstration purposes only — no
real transactions, courier bookings, or messages are processed or sent.
