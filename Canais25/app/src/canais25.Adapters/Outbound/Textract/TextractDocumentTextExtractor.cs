using Amazon.Textract;
using Amazon.Textract.Model;
using Canais25.Core.Ports.Out;
using System.Text;

namespace Canais25.Adapters.Outbound.Textract;

public class TextractDocumentTextExtractor : IDocumentTextExtractor
{
    private readonly IAmazonTextract _textract;

    public TextractDocumentTextExtractor(IAmazonTextract textract)
    {
        _textract = textract;
    }

    public async Task<string> ExtractTextAsync(string bucketName, string objectKey)
    {
        var request = new DetectDocumentTextRequest
        {
            Document = new Document
            {
                S3Object = new S3Object
                {
                    Bucket = bucketName,
                    Name = objectKey
                }
            }
        };

        var response = await _textract.DetectDocumentTextAsync(request);

        var text = new StringBuilder();

        foreach (var block in response.Blocks)
        {
            if (block.BlockType == BlockType.LINE)
            {
                text.AppendLine(block.Text);
            }
        }

        return text.ToString();
    }
}
