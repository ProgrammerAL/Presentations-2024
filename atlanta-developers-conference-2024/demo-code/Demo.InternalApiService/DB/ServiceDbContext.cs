#pragma warning disable IDE0058 // Expression value is never used

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Demo.InternalApiService.DB.EFEntities;

namespace Demo.InternalApiService.DB;

public class ServiceDbContext : DbContext
{
    public static readonly ImmutableArray<EventId> LoggingEventIds = new[]
    {
        RelationalEventId.CommandExecuted,
    }.ToImmutableArray();

    [NotNull]
    public DbSet<ProductEntity>? Products { get; private set; }

    public ServiceDbContext(DbContextOptions<ServiceDbContext> options)
        : base(options)
    {
    }
}
#pragma warning restore IDE0058 // Expression value is never used
