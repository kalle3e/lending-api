using LendingApi.Models;

namespace LendingApi.Data;

public static class DataSeeder
{
    public static void Seed(LendingContext db)
    {
        if (db.Indicators.Any()) return;

        var fragile          = new Indicator { Id = 1, Code = "FRAGILE",       Description = "Item is fragile - handling restrictions apply" };
        var accessRestricted = new Indicator { Id = 2, Code = "ACCESS_RESTR",  Description = "Access-restricted material" };
        var publicRequest    = new Indicator { Id = 3, Code = "PUBLIC_REQ",    Description = "Request made by a member of the public (virtual indicator)" };
        var researcherReq    = new Indicator { Id = 4, Code = "RESEARCHER_REQ",Description = "Request made by a registered researcher (virtual indicator)" };

        db.Indicators.AddRange(fragile, accessRestricted, publicRequest, researcherReq);

        var item1 = new Item { Id = 1, Title = "Ledger, 1923 - waterfront cargo manifests", Barcode = "LB000001" };
        var item2 = new Item { Id = 2, Title = "Photographic prints - city construction, 1950s", Barcode = "LB000002" };

        db.Items.AddRange(item1, item2);
        db.ItemIndicators.AddRange(
            new ItemIndicator { ItemId = 1, IndicatorId = accessRestricted.Id },
            new ItemIndicator { ItemId = 1, IndicatorId = fragile.Id },
            new ItemIndicator { ItemId = 2, IndicatorId = fragile.Id });

        db.IncompatibleIndicators.AddRange(
            new IncompatibleIndicator
            {
                Id = 1,
                IndicatorAId = accessRestricted.Id,
                IndicatorBId = publicRequest.Id,
                BlockMessage = "Access-restricted items cannot be lent on public requests. Refer to the access team."
            },
            new IncompatibleIndicator
            {
                Id = 2,
                IndicatorAId = fragile.Id,
                IndicatorBId = publicRequest.Id,
                BlockMessage = "Fragile items require supervised handling and are not available for public loan."
            });

        db.SaveChanges();
    }
}
