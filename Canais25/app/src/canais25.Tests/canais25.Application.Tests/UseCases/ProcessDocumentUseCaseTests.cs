using Canais25.Application.UseCases;
using Canais25.Core.Domain.Entities;
using Canais25.Core.Ports.In;
using Canais25.Core.Ports.Out;
using Moq;
using Xunit;

namespace Canais25.Tests.Application.UseCases
{
    public class ProcessDocumentUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_Deve_Extrair_Texto_Classificar_E_Salvar_No_Repositorio()
        {
            // Arrange
            var extractorMock = new Mock<IDocumentTextExtractor>();
            var categoryProviderMock = new Mock<ICategoryProvider>();
            var repositoryMock = new Mock<IComplaintRepository>();

            extractorMock
                .Setup(e => e.ExtractTextAsync("bucket-test", "doc.pdf"))
                .ReturnsAsync("texto com erro no app e cobrança indevida");

            categoryProviderMock
                .Setup(c => c.GetCategories())
                .Returns(new Dictionary<string, List<string>>
                {
                    { "app", new List<string> { "app", "erro" } },
                    { "cobranca", new List<string> { "cobrança", "indevida" } }
                });

            repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<ComplaintRecord>()))
                .Returns(Task.CompletedTask);

            var useCase = new ProcessDocumentUseCase(
                extractorMock.Object,
                categoryProviderMock.Object,
                repositoryMock.Object);

            // Act
            await useCase.ExecuteAsync("bucket-test", "doc.pdf");

            // Assert
            extractorMock.Verify(
                e => e.ExtractTextAsync("bucket-test", "doc.pdf"),
                Times.Once);

            categoryProviderMock.Verify(
                c => c.GetCategories(),
                Times.Once);

            repositoryMock.Verify(
                r => r.SaveAsync(It.Is<ComplaintRecord>(record =>
                    record.DocumentKey == "doc.pdf" &&
                    record.ExtractedText.Contains("erro") &&
                    record.Classifications.Contains("app") &&
                    record.Classifications.Contains("cobranca") &&
                    record.Status == "CLASSIFIED"
                )),
                Times.Once);
        }
    }
}
