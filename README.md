# OnlineShop

A multi-vendor marketplace — sellers open shops, list products, and message buyers; buyers browse,
favourite, review, and check out.

**Stack** — VB.NET · ASP.NET WebForms · Entity Framework · SQL Server


## Status

*Verified 28 Aug 2026 — builds with 0 errors; all six pages below served HTTP 200 under IIS Express.
`Database/schema.sql` applies to an empty database with no errors, creating all 17 tables.*

**Working**
- Storefront, product browse, product detail, category browse, shop search, and product search all render
- Full 17-table schema builds cleanly from source

**Needs setup**
- There is **no seed data** in this repository, so pages render their empty state. The schema is
  complete — you'd insert shops and products to see it populated.

**Not built**
- Checkout is modelled in the schema (`CartItems`, `Payments`, `Orders`, `OrderItems`) but there is
  no payment provider integration.

The value here is the domain model — a full marketplace including two-tier messaging
(buyer↔seller threads and shop-level enquiries) and view tracking.

## Domain

Thirteen tables covering the full marketplace: `Shops`, `Products`, `ProductImages`, `Categories`,
`CartItems`, `Payments`, `Reviews`, `Favorites`, `UserProductViews`, and a two-tier messaging model
(`Conversations` / `Messages` for buyer-seller threads, `ShopMessages` for shop-level enquiries).

## Pages

Storefront (`Default`), product browse and detail, category browse, shop detail, and two separate
search surfaces — product search and shop search.

## Running it locally

**Requires** Visual Studio 2022, .NET Framework 4.x, and SQL Server LocalDB (ships with Visual Studio).

1. Create the database and schema:
   ```
   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE OnlineShop"
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d OnlineShop -I -i Database/schema.sql
   ```
2. Open the solution in Visual Studio and press F5.

The connection string in `Web.config` points at LocalDB using Integrated Security — there are no
credentials in this repository, and none are required.
