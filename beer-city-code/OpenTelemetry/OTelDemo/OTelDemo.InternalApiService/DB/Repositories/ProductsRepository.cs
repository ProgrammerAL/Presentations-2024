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
    private readonly ServiceDbContext _context;

    public ProductsRepository(ServiceDbContext context)
    {
        _context = context;
    }

    public async ValueTask CreateProductAsync(string id, string name, int cost, string currencyCountry)
    {
        //using (var activity = ActivitySources.PurchasesServiceSource.StartActivity("CreateProduct"))
        {
            _ = await _context.Products.AddAsync(new ProductEntity
            {
                CreatedUtc = DateTime.UtcNow,
                Enabled = true,
                Id = id,
                Name = name,
                Cost = cost,
                CurrencyCountry = currencyCountry
            });

            _ = await _context.SaveChangesAsync();
        }
    }

    public async ValueTask<IReadOnlyCollection<ProductEntity>> AllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }
}
