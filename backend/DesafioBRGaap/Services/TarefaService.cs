using System.Text.Json;
using DesafioBRGaap.Models;

namespace DesafioBRGaap.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TarefaService> _logger;
        private readonly string _baseUrl = "https://jsonplaceholder.typicode.com";

        public TarefaService(HttpClient httpClient, ILogger<TarefaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Tarefa>> GetTarefasAsync(string? titleFilter = null)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas da API externa");

                var response = await _httpClient.GetAsync($"{_baseUrl}/todos");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var tarefas = JsonSerializer.Deserialize<List<Tarefa>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tarefas == null) return new List<Tarefa>();

                if (!string.IsNullOrEmpty(titleFilter))
                {
                    tarefas = tarefas.Where(t => t.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                    _logger.LogInformation("Filtradas {Count} tarefas por título: {Title}", tarefas.Count, titleFilter);
                }

                _logger.LogInformation("Retornando {Count} tarefas", tarefas.Count);
                return tarefas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas da API externa");
                throw new Exception($"Erro ao buscar tarefas: {ex.Message}", ex);
            }
        }

        public async Task<Tarefa?> GetTarefaByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando tarefa com ID: {Id}", id);

                var response = await _httpClient.GetAsync($"{_baseUrl}/todos/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var tarefa = JsonSerializer.Deserialize<Tarefa>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Tarefa encontrada: {Title}", tarefa?.Title);
                return tarefa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa com ID {Id}", id);
                throw new Exception($"Erro ao buscar tarefa com ID {id}: {ex.Message}", ex);
            }
        }
    }
}