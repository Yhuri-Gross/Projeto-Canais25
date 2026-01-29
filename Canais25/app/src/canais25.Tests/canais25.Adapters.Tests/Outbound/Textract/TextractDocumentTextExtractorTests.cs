using Amazon.Textract;
using Amazon.Textract.Model;
using Canais25.Adapters.Outbound.Textract;
using Moq;
using Xunit;

namespace Canais25.Tests.Adapters.Outbound.Textract
{
    public class TextractDocumentTextExtractorTests
    {
        [Fact]
        public async Task ExtractTextAsync_Deve_Extrair_Apenas_Linhas_Do_Documento()
        {
            var textractMock = new Mock<IAmazonTextract>();

            var response = new DetectDocumentTextResponse
            {
                Blocks = new List<Block>
                {
                    new Block { BlockType = BlockType.PAGE },
                    new Block { BlockType = BlockType.LINE, Text = "Primeira linha" },
                    new Block { BlockType = BlockType.WORD, Text = "Ignorado" },
                    new Block { BlockType = BlockType.LINE, Text = "Segunda linha" }
                }
            };

            textractMock
                .Setup(t => t.DetectDocumentTextAsync(
                    It.IsAny<DetectDocumentTextRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var extractor = new TextractDocumentTextExtractor(textractMock.Object);

            var result = await extractor.ExtractTextAsync("bucket-test", "file.pdf");

            Assert.Contains("Primeira linha", result);
            Assert.Contains("Segunda linha", result);
            Assert.DoesNotContain("Ignorado", result);

            textractMock.Verify(t => t.DetectDocumentTextAsync(
                It.Is<DetectDocumentTextRequest>(req =>
                    req.Document.S3Object.Bucket == "bucket-test" &&
                    req.Document.S3Object.Name == "file.pdf"
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
