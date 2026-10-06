using Microsoft.AspNetCore.Mvc;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;

namespace RestAPISistemSPK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AspeksController : ControllerBase
    {
        private readonly IKriteriaRepository _repo;

        public AspeksController(IKriteriaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllAspekAsync();
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repo.GetAspekByIdAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Aspek tidak ditemukan." });
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAspekDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NamaAspek))
                return BadRequest(new { success = false, message = "Nama aspek wajib diisi." });

            var result = await _repo.CreateAspekAsync(dto);
            return Ok(new { success = true, message = "Aspek evaluasi berhasil ditambahkan.", data = result });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAspekDto dto)
        {
            dto.Id = id;
            if (string.IsNullOrWhiteSpace(dto.NamaAspek))
                return BadRequest(new { success = false, message = "Nama aspek wajib diisi." });

            var result = await _repo.UpdateAspekAsync(dto);
            if (result == null) return NotFound(new { success = false, message = "Aspek tidak ditemukan." });

            return Ok(new { success = true, message = "Aspek evaluasi berhasil diperbarui.", data = result });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _repo.DeleteAspekAsync(id);
            if (!success) return NotFound(new { success = false, message = "Aspek tidak ditemukan." });

            return Ok(new { success = true, message = "Aspek dan parameter turunannya berhasil dihapus." });
        }
    }
}
