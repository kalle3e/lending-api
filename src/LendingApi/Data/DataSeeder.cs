using LendingApi.Models;

namespace LendingApi.Data;

public static class DataSeeder
{
    public static void Seed(LendingContext db)
    {
        if (db.Indicators.Any()) return;

        // No explicit Ids: SQL Server identity columns assign them.
        // Relationships are set by object reference so EF fixes up the keys.
        var fragile          = new Indicator { Code = "FRAGILE",        Description = "Item is fragile - handling restrictions apply" };
        var accessRestricted = new Indicator { Code = "ACCESS_RESTR",   Description = "Access-restricted material" };
        var publicRequest    = new Indicator { Code = "PUBLIC_REQ",     Description = "Request made by a member of the public (virtual indicator)" };
        var researcherReq    = new Indicator { Code = "RESEARCHER_REQ", Description = "Request made by a registered researcher (virtual indicator)" };

        db.Indicators.AddRange(fragile, accessRestricted, publicRequest, researcherReq);

        var item1 = new Item { Title = "Ledger, 1923 - waterfront cargo manifests", Barcode = "LB000001" };
        var item2 = new Item { Title = "Photographic prints - city construction, 1950s", Barcode = "LB000002" };

        item1.Indicators.Add(new ItemIndicator { Item = item1, Indicator = accessRestricted });
        item1.Indicators.Add(new ItemIndicator { Item = item1, Indicator = fragile });
        item2.Indicators.Add(new ItemIndicator { Item = item2, Indicator = fragile });

        db.Items.AddRange(item1, item2);

        // Indicators and items get their Ids here; the rules below need them.
        db.SaveChanges();

        db.IncompatibleIndicators.AddRange(
            new IncompatibleIndicator
            {
                IndicatorAId = accessRestricted.Id,
                IndicatorBId = publicRequest.Id,
                BlockMessage = "Access-restricted items cannot be lent on public requests. Refer to the access team."
            },
            new IncompatibleIndicator
            {
                IndicatorAId = fragile.Id,
                IndicatorBId = publicRequest.Id,
                BlockMessage = "Fragile items require supervised handling and are not available for public loan."
            });

        db.SaveChanges();
    }
}
