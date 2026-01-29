using Canais25.WebApi.Controllers;
using Canais25.WebApi.Controllers.Requests;
using Canais25.Core.Ports.In;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Canais25.Tests.WebApi.Controllers
{
    public class DocumentsControllerTests
    {
        [Fact]
        public async Task Process_Deve_Retornar_BadRequest_Quando_Request_Invalido()
        {
            // Arrange
            var processDocumentMock = new Mock<IProcessDocumentCommand>();
            var controller = new DocumentsController(processDocumentMock.Object);

            var request = new ProcessDocumentRequest
            {
                Bucket = "",
                Key = ""
            };

            // Act
            var result = await controller.Process(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bucket e Key são obrigatórios.", badRequest.Value);

            processDocumentMock.Verify(
                p => p.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Process_Deve_Executar_Command_E_Retornar_Accepted()
        {
            // Arrange
            var processDocumentMock = new Mock<IProcessDocumentCommand>();

            processDocumentMock
                .Setup(p => p.ExecuteAsync("bucket-test", "doc.pdf"))
                .Returns(Task.CompletedTask);

            var controller = new DocumentsController(processDocumentMock.Object);

            var request = new ProcessDocumentRequest
            {
                Bucket = "bucket-test",
                Key = "doc.pdf"
            };

            // Act
            var result = await controller.Process(request);

            // Assert
            var accepted = Assert.IsType<AcceptedResult>(result);
            Assert.NotNull(accepted.Value);

            processDocumentMock.Verify(
                p => p.ExecuteAsync("bucket-test", "doc.pdf"),
                Times.Once);
        }
    }
}
