using Microsoft.AspNetCore.Mvc;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;

namespace RestAPISistemSPK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _repo;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(ISupplierRepository repo, ILogger<SuppliersController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound(new { success = false, message = "Supplier tidak ditemukan." });
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NamaSupplier))
                return BadRequest(new { success = false, message = "Nama supplier wajib diisi." });

            var result = await _repo.CreateAsync(dto);
            return Ok(new { success = true, message = "Supplier berhasil ditambahkan.", data = result });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierDto dto)
        {
            dto.Id = id;
            if (string.IsNullOrWhiteSpace(dto.NamaSupplier))
                return BadRequest(new { success = false, message = "Nama supplier wajib diisi." });

            var result = await _repo.UpdateAsync(dto);
            if (result == null) return NotFound(new { success = false, message = "Supplier tidak ditemukan." });

            return Ok(new { success = true, message = "Supplier berhasil diperbarui.", data = result });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success) return NotFound(new { success = false, message = "Supplier tidak ditemukan." });

            return Ok(new { success = true, message = "Supplier berhasil dihapus/dinonaktifkan." });
        }
    }
}
