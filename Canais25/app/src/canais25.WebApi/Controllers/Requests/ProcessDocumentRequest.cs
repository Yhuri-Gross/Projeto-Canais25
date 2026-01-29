namespace Canais25.WebApi.Controllers.Requests;

public class ProcessDocumentRequest
{
    public string Bucket { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
