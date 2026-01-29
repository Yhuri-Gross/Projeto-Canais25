using Core.Ports.In;
using Canais25.WebApi.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Canais25.WebApi.Controllers;

[ApiController]
[Route("api/complaints")]
public class ComplaintsController : ControllerBase
{
    private readonly IProcessComplaintUseCase _useCase;

    public ComplaintsController(IProcessComplaintUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Process([FromBody] ProcessComplaintRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Id) ||
            string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Id e Description são obrigatórios.");
        }

        await _useCase.ExecuteAsync(request.Id, request.Description);

        return Created(string.Empty, new
        {
            message = "Reclamação processada com sucesso",
            id = request.Id
        });
    }
}
