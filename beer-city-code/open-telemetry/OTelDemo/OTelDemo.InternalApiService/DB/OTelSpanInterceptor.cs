using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;

using OpenTelemetry.Trace;

namespace OTelDemo.InternalApiService.DB;

public class OTelSpanInterceptor : DbCommandInterceptor
{
    public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        Tracer.CurrentSpan.SetAttribute("my-custom.db.statement", command.CommandText);
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        Tracer.CurrentSpan.SetAttribute("my-custom.db.statement", command.CommandText);
        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
}
