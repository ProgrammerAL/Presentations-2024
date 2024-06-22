using System.Net;
using System.Text;

using FeedbackFunctionsApp.Persistence;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FeedbackFunctionsApp.Functions.StoreComment;

public class StoreCommentsFunction
{
    private readonly IAzTablePersister _tablePersister;
    private readonly ISqlPersister _sqlPersister;

    public StoreCommentsFunction(IAzTablePersister tablePersister, ISqlPersister sqlPersister)
    {
        _tablePersister = tablePersister;
        _sqlPersister = sqlPersister;
    }

    [Function("store-comments")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var requestDto = await req.ReadFromJsonAsync<StoreCommentsRequestDto>();
        if (requestDto is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var requestObject = requestDto.GenerateValidObject();
        //await _tablePersister.StoreItemAsync(requestObject.Comments);
        await _sqlPersister.StoreItemAsync(requestObject.Comments);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
