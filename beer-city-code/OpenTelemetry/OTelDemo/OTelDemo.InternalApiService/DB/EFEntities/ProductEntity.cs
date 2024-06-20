using System.ComponentModel.DataAnnotations;

namespace OTelDemo.InternalApiService.DB.EFEntities;

public class ProductEntity
{
    [Key]
    public required string Id { get; set; }
    public required DateTime CreatedUtc { get; set; }
    public required bool Enabled { get; set; }
    public required string Name { get; set; }
    public required int Cost { get; set; }
    public required string CurrencyCountry { get; set; }
}

