using System.Collections.Immutable;

using Microsoft.EntityFrameworkCore;

using OTelDemo.InternalApiService.DB.EFEntities;

namespace OTelDemo.InternalApiService.DB.Repositories;

public interface IProductsRepository
{
    ValueTask CreateProductAsync(string id, string name, int cost, string currencyCountry);
    ValueTask<IReadOnlyCollection<ProductEntity>> AllProductsAsync();
}

public class ProductsRepository : IProductsRepository
{
    private readonly IDbContextFactory<ServiceDbContext> _contextFactory;

    public ProductsRepository(IDbContextFactory<ServiceDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask CreateProductAsync(string id, string name, int cost, string currencyCountry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        //using (var activity = ActivitySources.PurchasesServiceSource.StartActivity("CreateProduct"))
        {
            _ = await context.Products.AddAsync(new ProductEntity
            {
                CreatedUtc = DateTime.UtcNow,
                Enabled = true,
                Id = id,
                Name = name,
                Cost = cost,
                CurrencyCountry = currencyCountry
            });

            _ = await context.SaveChangesAsync();
        }
    }

    public async ValueTask<IReadOnlyCollection<ProductEntity>> AllProductsAsync()
    { 
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products.ToListAsync();
    }
}
