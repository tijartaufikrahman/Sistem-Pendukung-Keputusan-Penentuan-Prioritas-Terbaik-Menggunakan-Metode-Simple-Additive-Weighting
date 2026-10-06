using Microsoft.AspNetCore.Mvc;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;

namespace RestAPISistemSPK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeputusansController : ControllerBase
    {
        private readonly IKeputusanRepository _repo;
        private readonly ILogger<KeputusansController> _logger;

        public KeputusansController(IKeputusanRepository repo, ILogger<KeputusansController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var summary = await _repo.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
        {
            var list = await _repo.GetAllAsync(status);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Sesi keputusan tidak ditemukan." });
            return Ok(item);
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] CalculateKeputusanRequestDto request)
        {
            if (request.SupplierIds == null || request.SupplierIds.Count < 2)
                return BadRequest(new { success = false, message = "Pilih minimal 2 supplier untuk dievaluasi." });

            if (request.ParameterIds == null || request.ParameterIds.Count < 1)
                return BadRequest(new { success = false, message = "Pilih minimal 1 parameter evaluasi." });

            var result = await _repo.CalculateAsync(request);
            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveKeputusanRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NamaKeputusan))
                dto.NamaKeputusan = "Sesi Penilaian Alternatif " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var result = await _repo.SaveAsync(dto);
            var isDraft = (result.Status == "Draft");
            return Ok(new { 
                success = true, 
                message = isDraft ? "Draf sesi penilaian berhasil disimpan." : "Hasil penentuan prioritas alternatif berhasil disimpan.", 
                id = result.Id, 
                kode = result.KodeKeputusan,
                status = result.Status
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success) return NotFound(new { success = false, message = "Sesi penilaian alternatif tidak ditemukan." });

            return Ok(new { success = true, message = "Riwayat penentuan prioritas alternatif berhasil dihapus." });
        }
    }
}
