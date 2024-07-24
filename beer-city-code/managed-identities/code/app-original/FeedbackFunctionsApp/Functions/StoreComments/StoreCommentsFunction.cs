using System.Net;
using System.Text;

using FeedbackFunctionsApp.Persistence;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FeedbackFunctionsApp.Functions.StoreComment;

public class StoreCommentsFunction
{
    private readonly IAzTablePersister _tablePersister;

    public StoreCommentsFunction(IAzTablePersister tablePersister)
    {
        _tablePersister = tablePersister;
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
        await _tablePersister.StoreItemAsync(requestObject.Comments);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
