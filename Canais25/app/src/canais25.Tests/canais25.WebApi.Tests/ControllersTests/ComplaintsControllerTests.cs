using Canais25.WebApi.Controllers;
using Canais25.WebApi.Controllers.Requests;
using Core.Ports.In;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Canais25.Tests.WebApi.Controllers
{
    public class ComplaintsControllerTests
    {
        [Fact]
        public async Task Process_Deve_Retornar_BadRequest_Quando_Request_Invalido()
        {
            // Arrange
            var useCaseMock = new Mock<IProcessComplaintUseCase>();
            var controller = new ComplaintsController(useCaseMock.Object);

            var request = new ProcessComplaintRequest
            {
                Id = "",
                Description = ""
            };

            // Act
            var result = await controller.Process(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id e Description são obrigatórios.", badRequest.Value);

            useCaseMock.Verify(
                u => u.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Process_Deve_Processar_Reclamacao_E_Retornar_Created()
        {
            // Arrange
            var useCaseMock = new Mock<IProcessComplaintUseCase>();

            useCaseMock
                .Setup(u => u.ExecuteAsync("cmp-123", "descrição válida"))
                .Returns(Task.CompletedTask);

            var controller = new ComplaintsController(useCaseMock.Object);

            var request = new ProcessComplaintRequest
            {
                Id = "cmp-123",
                Description = "descrição válida"
            };

            // Act
            var result = await controller.Process(request);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(createdResult.Value);

            useCaseMock.Verify(
                u => u.ExecuteAsync("cmp-123", "descrição válida"),
                Times.Once);
        }
    }
}
