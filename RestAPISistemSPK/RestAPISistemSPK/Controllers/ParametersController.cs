using Microsoft.AspNetCore.Mvc;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;

namespace RestAPISistemSPK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametersController : ControllerBase
    {
        private readonly IKriteriaRepository _repo;

        public ParametersController(IKriteriaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? aspekId)
        {
            var data = await _repo.GetAllParameterAsync(aspekId);
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repo.GetParameterByIdAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Parameter tidak ditemukan." });
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParameterDto dto)
        {
            if (dto.AspekId == Guid.Empty)
                return BadRequest(new { success = false, message = "Pilih aspek evaluasi terlebih dahulu." });
            if (string.IsNullOrWhiteSpace(dto.NamaParameter))
                return BadRequest(new { success = false, message = "Nama parameter wajib diisi." });
            if (string.IsNullOrWhiteSpace(dto.Satuan))
                return BadRequest(new { success = false, message = "Satuan ukuran wajib diisi." });

            var result = await _repo.CreateParameterAsync(dto);
            return Ok(new { success = true, message = "Parameter evaluasi berhasil ditambahkan.", data = result });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateParameterDto dto)
        {
            dto.Id = id;
            if (dto.AspekId == Guid.Empty)
                return BadRequest(new { success = false, message = "Pilih aspek evaluasi terlebih dahulu." });
            if (string.IsNullOrWhiteSpace(dto.NamaParameter))
                return BadRequest(new { success = false, message = "Nama parameter wajib diisi." });

            var result = await _repo.UpdateParameterAsync(dto);
            if (result == null) return NotFound(new { success = false, message = "Parameter tidak ditemukan." });

            return Ok(new { success = true, message = "Parameter evaluasi berhasil diperbarui.", data = result });
        }

        [HttpPut("batch-bobot")]
        public async Task<IActionResult> UpdateBatchBobot([FromBody] List<ParameterBobotItemDto> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest(new { success = false, message = "Data bobot kriteria tidak boleh kosong." });

            var success = await _repo.UpdateBatchBobotAsync(items);
            if (!success)
                return BadRequest(new { success = false, message = "Gagal memperbarui bobot kriteria." });

            return Ok(new { success = true, message = "Pengaturan bobot prioritas kriteria berhasil disimpan ke database." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _repo.DeleteParameterAsync(id);
            if (!success) return NotFound(new { success = false, message = "Parameter tidak ditemukan." });

            return Ok(new { success = true, message = "Parameter berhasil dihapus." });
        }
    }
}
