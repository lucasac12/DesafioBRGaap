using Microsoft.AspNetCore.Mvc;
using DesafioBRGaap.Models;
using DesafioBRGaap.Services;

namespace DesafioBRGaap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITarefaService tarefaService, ILogger<TodosController> logger)
        {
            _tarefaService = tarefaService;
            _logger = logger;
        }

        /// <summary>
        /// GET /todos → retorna uma lista de tarefas
        /// </summary>
        /// <param name="title">Filtro opcional por título (ex: GET /todos?title=expedita)</param>
        /// <returns>Lista de tarefas</returns>
        [HttpGet]
        public async Task<ActionResult<List<Tarefa>>> GetTodos([FromQuery] string? title = null)
        {
            try
            {
                _logger.LogInformation("GET /todos - Filtro: {Title}", title ?? "Nenhum");

                var tarefas = await _tarefaService.GetTarefasAsync(title);

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos");
                return StatusCode(500, new
                {
                    message = "Erro interno do servidor ao buscar todos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// GET /todos/{id} → retorna os detalhes de uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Detalhes da tarefa</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> GetTodo(int id)
        {
            try
            {
                _logger.LogInformation("GET /todos/{Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new { message = "ID deve ser maior que zero" });
                }

                var tarefa = await _tarefaService.GetTarefaByIdAsync(id);

                if (tarefa == null)
                {
                    return NotFound(new { message = $"Todo com ID {id} não encontrado" });
                }

                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todo com ID {Id}", id);
                return StatusCode(500, new
                {
                    message = $"Erro interno do servidor ao buscar todo com ID {id}",
                    error = ex.Message
                });
            }
        }
    }
}