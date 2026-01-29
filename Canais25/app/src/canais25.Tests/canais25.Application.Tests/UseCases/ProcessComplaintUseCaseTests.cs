using Application.UseCases;
using Core.Domain.Entities;
using Core.Ports.In;
using Core.Ports.Out;
using Moq;
using Xunit;

namespace Application.Tests.UseCases
{
    public class ProcessComplaintUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_Deve_Classificar_Complaint_E_Salvar_No_Repositorio()
        {
            // Arrange
            var repositoryMock = new Mock<IComplaintRepository>();
            var classifierMock = new Mock<IClassifier>();

            classifierMock
                .Setup(c => c.Classify("descrição de teste"))
                .Returns(new List<string> { "fraude", "cobrança" });

            repositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<Complaint>()))
                .Returns(Task.CompletedTask);

            var useCase = new ProcessComplaintUseCase(
                repositoryMock.Object,
                classifierMock.Object);

            // Act
            await useCase.ExecuteAsync("cmp-123", "descrição de teste");

            // Assert
            classifierMock.Verify(
                c => c.Classify("descrição de teste"),
                Times.Once);

            repositoryMock.Verify(
                r => r.SaveAsync(It.Is<Complaint>(complaint =>
                    complaint.Id == "cmp-123" &&
                    complaint.Description == "descrição de teste" &&
                    complaint.Categories.Contains("fraude") &&
                    complaint.Categories.Contains("cobrança")
                )),
                Times.Once);
        }
    }
}
