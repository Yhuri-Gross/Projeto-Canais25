using Canais25.Application.UseCases;
using Canais25.Core.Ports.Out;
using Moq;
using Xunit;

namespace Canais25.Tests.Application.UseCases
{
    public class ProcessComplaintFromDocumentUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_Deve_Chamar_Extractor_E_Retornar_Texto()
        {
            var extractorMock = new Mock<IDocumentTextExtractor>();

            extractorMock
                .Setup(e => e.ExtractTextAsync("bucket-test", "file.pdf"))
                .ReturnsAsync("texto extraído");

            var useCase = new ProcessComplaintFromDocumentUseCase(extractorMock.Object);

            var result = await useCase.ExecuteAsync("bucket-test", "file.pdf");

            Assert.Equal("texto extraído", result);

            extractorMock.Verify(
                e => e.ExtractTextAsync("bucket-test", "file.pdf"),
                Times.Once);
        }
    }
}
