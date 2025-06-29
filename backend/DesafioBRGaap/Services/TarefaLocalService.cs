using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DesafioBRGaap.Models;

namespace DesafioBRGaap.Services
{
    public class TarefaLocalService : ITarefaLocalService
    {
        private readonly TarefaContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TarefaLocalService> _logger;
        private readonly string _baseUrl = "https://jsonplaceholder.typicode.com";

        public TarefaLocalService(TarefaContext context, HttpClient httpClient, ILogger<TarefaLocalService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Tarefa>> GetTarefasAsync(string? titleFilter = null)
        {
            try
            {
                if (!await TemDadosLocaisAsync())
                {
                    _logger.LogInformation("Banco local vazio. Sincronizando dados da API externa...");
                    await SincronizarDadosAsync();
                }

                IQueryable<Tarefa> query = _context.Tarefas;

                if (!string.IsNullOrEmpty(titleFilter))
                {
                    query = query.Where(t => t.Title.Contains(titleFilter));
                    _logger.LogInformation("Aplicando filtro por título: {Title}", titleFilter);
                }

                var tarefas = await query.ToListAsync();
                _logger.LogInformation("Retornando {Count} tarefas do banco local", tarefas.Count);

                return tarefas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas do banco local");
                throw new Exception($"Erro ao buscar tarefas: {ex.Message}", ex);
            }
        }

        public async Task<Tarefa?> GetTarefaByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando tarefa com ID: {Id} no banco local", id);

                if (!await TemDadosLocaisAsync())
                {
                    _logger.LogInformation("Banco local vazio. Sincronizando dados da API externa...");
                    await SincronizarDadosAsync();
                }

                var tarefa = await _context.Tarefas.FindAsync(id);

                if (tarefa != null)
                {
                    _logger.LogInformation("Tarefa encontrada no banco local: {Title}", tarefa.Title);
                }
                else
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada no banco local", id);
                }

                return tarefa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa com ID {Id} no banco local", id);
                throw new Exception($"Erro ao buscar tarefa com ID {id}: {ex.Message}", ex);
            }
        }

        public async Task SincronizarDadosAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronização de dados da API externa");

                var response = await _httpClient.GetAsync($"{_baseUrl}/todos");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var tarefasExternas = JsonSerializer.Deserialize<List<Tarefa>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tarefasExternas == null || !tarefasExternas.Any())
                {
                    _logger.LogWarning("Nenhuma tarefa encontrada na API externa");
                    return;
                }

                await LimparDadosLocaisAsync();

                await _context.Tarefas.AddRangeAsync(tarefasExternas);
                var linhasAfetadas = await _context.SaveChangesAsync();

                _logger.LogInformation("Sincronização concluída. {Count} tarefas salvas no banco local", linhasAfetadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante sincronização de dados");
                throw new Exception($"Erro na sincronização: {ex.Message}", ex);
            }
        }

        public async Task ForceSyncAsync()
        {
            _logger.LogInformation("Forçando nova sincronização");
            await SincronizarDadosAsync();
        }

        public async Task<bool> TemDadosLocaisAsync()
        {
            var count = await _context.Tarefas.CountAsync();
            return count > 0;
        }

        public async Task LimparDadosLocaisAsync()
        {
            var tarefasExistentes = await _context.Tarefas.ToListAsync();
            if (tarefasExistentes.Any())
            {
                _context.Tarefas.RemoveRange(tarefasExistentes);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Removidas {Count} tarefas do banco local", tarefasExistentes.Count);
            }
        }

        public async Task<object> GetEstatisticasAsync()
        {
            var totalTarefas = await _context.Tarefas.CountAsync();
            var tarefasConcluidas = await _context.Tarefas.CountAsync(t => t.Completed);
            var tarefasPendentes = totalTarefas - tarefasConcluidas;
            var usuariosUnicos = await _context.Tarefas.Select(t => t.UserId).Distinct().CountAsync();

            return new
            {
                TotalTarefas = totalTarefas,
                TarefasConcluidas = tarefasConcluidas,
                TarefasPendentes = tarefasPendentes,
                UsuariosUnicos = usuariosUnicos,
                UltimaAtualizacao = DateTime.UtcNow
            };
        }
    }
}