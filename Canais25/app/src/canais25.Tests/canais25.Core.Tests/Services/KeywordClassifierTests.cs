using Canais25.Core.Domain.Entities;
using Canais25.Core.Domain.Services;
using Xunit;

namespace Canais25.Tests.Core.Domain.Services
{
    public class KeywordClassifierTests
    {
        [Fact]
        public void Classify_Deve_Classificar_E_Ordenar_Por_Quantidade_De_Matches()
        {
            // Arrange
            var classifier = new KeywordClassifier();

            var categories = new Dictionary<string, List<string>>
            {
                { "app", new List<string> { "app", "erro" } },
                { "cobranca", new List<string> { "cobrança", "indevida", "valor" } },
                { "acesso", new List<string> { "login", "senha" } }
            };

            var text = "O app apresenta erro e cobrança indevida com valor errado";

            // Act
            var result = classifier.Classify(text, categories);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal("cobranca", result[0].Category);
            Assert.Equal(2, result[0].MatchCount);

            Assert.Equal("app", result[1].Category);
            Assert.Equal(2, result[1].MatchCount);
        }

        [Fact]
        public void Classify_Deve_Ignorar_Categorias_Sem_Match()
        {
            // Arrange
            var classifier = new KeywordClassifier();

            var categories = new Dictionary<string, List<string>>
            {
                { "fraude", new List<string> { "fraude" } }
            };

            var text = "Texto sem palavras relevantes";

            // Act
            var result = classifier.Classify(text, categories);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Classify_Deve_Ser_Case_Insensitive()
        {
            // Arrange
            var classifier = new KeywordClassifier();

            var categories = new Dictionary<string, List<string>>
            {
                { "app", new List<string> { "App", "Erro" } }
            };

            var text = "APP COM ERRO AO ABRIR";

            // Act
            var result = classifier.Classify(text, categories);

            // Assert
            Assert.Single(result);
            Assert.Equal("app", result[0].Category);
            Assert.Equal(2, result[0].MatchCount);
        }
    }
}
