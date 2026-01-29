using Canais25.Core.Ports.In;
using Canais25.WebApi.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Canais25.WebApi.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IProcessDocumentCommand _processDocument;

    public DocumentsController(IProcessDocumentCommand processDocument)
    {
        _processDocument = processDocument;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] ProcessDocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Bucket) ||
            string.IsNullOrWhiteSpace(request.Key))
        {
            return BadRequest("Bucket e Key são obrigatórios.");
        }

        await _processDocument.ExecuteAsync(request.Bucket, request.Key);

        return Accepted(new
        {
            message = "Documento recebido para processamento",
            bucket = request.Bucket,
            key = request.Key
        });
    }
}
