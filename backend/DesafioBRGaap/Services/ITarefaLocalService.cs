using DesafioBRGaap.Models;

namespace DesafioBRGaap.Services
{
    public interface ITarefaLocalService : ITarefaService
    {
        /// <summary>
        /// Sincroniza dados da API externa para o banco local
        /// </summary>
        Task SincronizarDadosAsync();

        /// <summary>
        /// Força uma nova sincronização completa
        /// </summary>
        Task ForceSyncAsync();

        /// <summary>
        /// Verifica se o banco local tem dados
        /// </summary>
        Task<bool> TemDadosLocaisAsync();

        /// <summary>
        /// Limpa todos os dados locais
        /// </summary>
        Task LimparDadosLocaisAsync();

        /// <summary>
        /// Retorna estatísticas do banco local
        /// </summary>
        Task<object> GetEstatisticasAsync();
    }
}