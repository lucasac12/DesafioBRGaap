using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using DesafioBRGaap.Models;
using DesafioBRGaap.Services;

namespace DesafioBRGaap.Tests
{
    public class TarefaServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<TarefaService>> _loggerMock;
        private readonly TarefaService _tarefaService;

        public TarefaServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<TarefaService>>();
            _tarefaService = new TarefaService(_httpClient, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTarefasAsync_SemFiltro_DeveRetornarListaDeTarefas()
        {
            var responseJson = """
                [
                    {
                        "id": 1,
                        "userId": 1,
                        "title": "delectus aut autem",
                        "completed": false
                    },
                    {
                        "id": 2,
                        "userId": 1,
                        "title": "quis ut nam facilis et officia qui",
                        "completed": false
                    }
                ]
                """;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var result = await _tarefaService.GetTarefasAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("delectus aut autem", result[0].Title);
            Assert.Equal(1, result[0].UserId);
            Assert.False(result[0].Completed);
        }

        [Fact]
        public async Task GetTarefasAsync_ComFiltroTitulo_DeveRetornarTarefasFiltradas()
        {
            var responseJson = """
                [
                    {
                        "id": 1,
                        "userId": 1,
                        "title": "delectus aut autem",
                        "completed": false
                    },
                    {
                        "id": 2,
                        "userId": 1,
                        "title": "quis ut nam facilis et officia qui",
                        "completed": false
                    }
                ]
                """;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var result = await _tarefaService.GetTarefasAsync("delectus");

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("delectus aut autem", result[0].Title);
        }

        [Fact]
        public async Task GetTarefaByIdAsync_TarefaExiste_DeveRetornarTarefa()
        {
            var responseJson = """
                {
                    "id": 1,
                    "userId": 1,
                    "title": "delectus aut autem",
                    "completed": false
                }
                """;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var result = await _tarefaService.GetTarefaByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("delectus aut autem", result.Title);
            Assert.Equal(1, result.UserId);
            Assert.False(result.Completed);
        }

        [Fact]
        public async Task GetTarefaByIdAsync_TarefaNaoExiste_DeveRetornarNull()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _tarefaService.GetTarefaByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTarefasAsync_ErroNaAPI_DeveLancarException()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _tarefaService.GetTarefasAsync());
            Assert.Contains("Erro ao buscar tarefas", exception.Message);
        }
    }
}