using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DesafioBRGaap.Controllers;
using DesafioBRGaap.Models;
using DesafioBRGaap.Services;

namespace DesafioBRGaap.Tests
{
    public class TodosControllerTests
    {
        private readonly Mock<ITarefaService> _tarefaServiceMock;
        private readonly Mock<ILogger<TodosController>> _loggerMock;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _tarefaServiceMock = new Mock<ITarefaService>();
            _loggerMock = new Mock<ILogger<TodosController>>();
            _controller = new TodosController(_tarefaServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTodos_SemFiltro_DeveRetornar200ComListaDeTarefas()
        {
            var tarefasEsperadas = new List<Tarefa>
            {
                new Tarefa { Id = 1, UserId = 1, Title = "Tarefa 1", Completed = false },
                new Tarefa { Id = 2, UserId = 1, Title = "Tarefa 2", Completed = true }
            };

            _tarefaServiceMock
                .Setup(s => s.GetTarefasAsync(null))
                .ReturnsAsync(tarefasEsperadas);

            var result = await _controller.GetTodos();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tarefas = Assert.IsType<List<Tarefa>>(okResult.Value);
            Assert.Equal(2, tarefas.Count);
            Assert.Equal("Tarefa 1", tarefas[0].Title);
        }

        [Fact]
        public async Task GetTodos_ComFiltro_DeveRetornar200ComTarefasFiltradas()
        {
            var tarefasFiltradas = new List<Tarefa>
            {
                new Tarefa { Id = 1, UserId = 1, Title = "Tarefa expedita", Completed = false }
            };

            _tarefaServiceMock
                .Setup(s => s.GetTarefasAsync("expedita"))
                .ReturnsAsync(tarefasFiltradas);

            var result = await _controller.GetTodos("expedita");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tarefas = Assert.IsType<List<Tarefa>>(okResult.Value);
            Assert.Single(tarefas);
            Assert.Contains("expedita", tarefas[0].Title);
        }

        [Fact]
        public async Task GetTodo_ComIdValido_DeveRetornar200ComTarefa()
        {
            var tarefaEsperada = new Tarefa
            {
                Id = 1,
                UserId = 1,
                Title = "Tarefa 1",
                Completed = false
            };

            _tarefaServiceMock
                .Setup(s => s.GetTarefaByIdAsync(1))
                .ReturnsAsync(tarefaEsperada);

            var result = await _controller.GetTodo(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tarefa = Assert.IsType<Tarefa>(okResult.Value);
            Assert.Equal(1, tarefa.Id);
            Assert.Equal("Tarefa 1", tarefa.Title);
        }

        [Fact]
        public async Task GetTodo_ComIdInvalido_DeveRetornar400()
        {
            var result = await _controller.GetTodo(0);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetTodo_TarefaNaoEncontrada_DeveRetornar404()
        {
            _tarefaServiceMock
                .Setup(s => s.GetTarefaByIdAsync(999))
                .ReturnsAsync((Tarefa?)null);

            var result = await _controller.GetTodo(999);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetTodos_ErroNoService_DeveRetornar500()
        {
            _tarefaServiceMock
                .Setup(s => s.GetTarefasAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro interno"));

            var result = await _controller.GetTodos();

            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}