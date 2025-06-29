using Microsoft.Extensions.Logging;
using Moq;
using DesafioBRGaap.Models;
using DesafioBRGaap.Services;

namespace DesafioBRGaap.Tests
{
    public class TarefaLocalServiceTests
    {
        [Fact]
        public void TarefaLocalService_DeveTerInterfaceCorreta()
        {
            // Teste para verificar se a interface está correta
            Assert.True(typeof(ITarefaLocalService).IsAssignableFrom(typeof(TarefaLocalService)));
            Assert.True(typeof(ITarefaService).IsAssignableFrom(typeof(TarefaLocalService)));
        }

        [Fact]
        public void TarefaContext_DevePermitirInstanciacao()
        {
            // Teste para verificar se o TarefaContext está correto
            Assert.True(typeof(TarefaContext).IsSubclassOf(typeof(Microsoft.EntityFrameworkCore.DbContext)));
        }
    }
}