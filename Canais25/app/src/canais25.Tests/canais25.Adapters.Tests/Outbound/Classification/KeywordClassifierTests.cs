using Core.Domain.Services;
using Xunit;

namespace Core.Tests.Domain.Services
{
    public class KeywordClassifierTests
    {
        [Fact]
        public void Classify_Deve_Classificar_Fraude()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("Não reconheço essa compra no meu cartão");

            Assert.Contains("fraude", result);
        }

        [Fact]
        public void Classify_Deve_Classificar_Cobranca()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("Minha fatura veio com valor indevido");

            Assert.Contains("cobrança", result);
        }

        [Fact]
        public void Classify_Deve_Classificar_Acesso()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("Não consigo acessar minha conta, esqueci a senha");

            Assert.Contains("acesso", result);
        }

        [Fact]
        public void Classify_Deve_Classificar_App()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("O aplicativo está com erro ao abrir");

            Assert.Contains("app", result);
        }

        [Fact]
        public void Classify_Deve_Retornar_Multiplas_Categorias()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("Erro no app e cobrança indevida na fatura").ToList();

            Assert.Contains("app", result);
            Assert.Contains("cobrança", result);
        }

        [Fact]
        public void Classify_Nao_Deve_Classificar_Quando_Nao_Ha_Palavras_Chave()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("Gostaria apenas de informações gerais");

            Assert.Empty(result);
        }

        [Fact]
        public void Classify_Deve_Ser_Case_Insensitive()
        {
            var classifier = new KeywordClassifier();

            var result = classifier.Classify("APP COM ERRO NO LOGIN");

            Assert.Contains("app", result);
            Assert.Contains("acesso", result);
        }
    }
}
