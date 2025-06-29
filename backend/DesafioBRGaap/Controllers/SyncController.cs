using Microsoft.AspNetCore.Mvc;
using DesafioBRGaap.Services;

namespace DesafioBRGaap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly ITarefaLocalService _tarefaLocalService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(ITarefaLocalService tarefaService, ILogger<SyncController> logger)
        {
            _tarefaLocalService = tarefaService;
            _logger = logger;
        }

        /// <summary>
        /// Força sincronização manual dos dados
        /// </summary>
        [HttpPost("force")]
        public async Task<IActionResult> ForceSyncAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronização manual");

                await _tarefaLocalService.ForceSyncAsync();

                return Ok(new
                {
                    message = "Sincronização realizada com sucesso",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante sincronização manual");
                return StatusCode(500, new
                {
                    message = "Erro durante sincronização",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Verifica status da sincronização
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetSyncStatus()
        {
            try
            {
                var temDados = await _tarefaLocalService.TemDadosLocaisAsync();
                var estatisticas = await _tarefaLocalService.GetEstatisticasAsync();

                return Ok(new
                {
                    status = "Sistema configurado para persistência local",
                    database = "SQLite",
                    temDadosLocais = temDados,
                    estatisticas,
                    endpoints = new
                    {
                        forceSyncUrl = "/api/sync/force",
                        clearDataUrl = "/api/sync/clear"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status");
                return StatusCode(500, new { message = "Erro ao verificar status", error = ex.Message });
            }
        }

        /// <summary>
        /// Limpa todos os dados locais
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearLocalData()
        {
            try
            {
                await _tarefaLocalService.LimparDadosLocaisAsync();

                return Ok(new
                {
                    message = "Dados locais limpos com sucesso",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar dados locais");
                return StatusCode(500, new
                {
                    message = "Erro ao limpar dados locais",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Retorna estatísticas do banco local
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var estatisticas = await _tarefaLocalService.GetEstatisticasAsync();
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas");
                return StatusCode(500, new { message = "Erro ao buscar estatísticas", error = ex.Message });
            }
        }
    }
}