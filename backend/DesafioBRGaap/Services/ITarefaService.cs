using DesafioBRGaap.Models;

namespace DesafioBRGaap.Services
{
    public interface ITarefaService
    {
        Task<List<Tarefa>> GetTarefasAsync(string? titleFilter = null);
        Task<Tarefa?> GetTarefaByIdAsync(int id);
    }
}