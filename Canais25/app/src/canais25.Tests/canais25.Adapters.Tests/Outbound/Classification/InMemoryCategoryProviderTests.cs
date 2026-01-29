using Canais25.Adapters.Outbound.Classification;
using Xunit;

namespace Canais25.Tests.Adapters.Outbound.Classification
{
    public class InMemoryCategoryProviderTests
    {
        [Fact]
        public void GetCategories_Deve_Retornar_Todas_As_Categorias_Com_Palavras_Chave()
        {
            // Arrange
            var provider = new InMemoryCategoryProvider();

            // Act
            var categories = provider.GetCategories();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(6, categories.Count);

            Assert.Contains("imobiliario", categories.Keys);
            Assert.Contains("seguros", categories.Keys);
            Assert.Contains("cobranca", categories.Keys);
            Assert.Contains("acesso", categories.Keys);
            Assert.Contains("aplicativo", categories.Keys);
            Assert.Contains("fraude", categories.Keys);

            Assert.Contains("credito imobiliario", categories["imobiliario"]);
            Assert.Contains("casa", categories["imobiliario"]);
            Assert.Contains("apartamento", categories["imobiliario"]);

            Assert.Contains("resgate", categories["seguros"]);
            Assert.Contains("capitalizacao", categories["seguros"]);
            Assert.Contains("socorro", categories["seguros"]);

            Assert.Contains("fatura", categories["cobranca"]);
            Assert.Contains("cobranca", categories["cobranca"]);
            Assert.Contains("valor", categories["cobranca"]);
            Assert.Contains("indevido", categories["cobranca"]);

            Assert.Contains("acessar", categories["acesso"]);
            Assert.Contains("login", categories["acesso"]);
            Assert.Contains("senha", categories["acesso"]);

            Assert.Contains("app", categories["aplicativo"]);
            Assert.Contains("aplicativo", categories["aplicativo"]);
            Assert.Contains("travando", categories["aplicativo"]);
            Assert.Contains("erro", categories["aplicativo"]);

            Assert.Contains("nao reconhece divida", categories["fraude"]);
            Assert.Contains("fraude", categories["fraude"]);
        }
    }
}
