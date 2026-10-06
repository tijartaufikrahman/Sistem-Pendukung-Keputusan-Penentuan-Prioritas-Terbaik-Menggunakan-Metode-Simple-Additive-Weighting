using Microsoft.AspNetCore.Mvc;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserRepository userRepo,
            ILogger<UsersController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _userRepo.CountAsync();
            return Ok(new { count });
        }

        [HttpGet]
        [HttpGet("/api/admin/GetUsers")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAllAsync();
            var result = users
                .OrderBy(u => u.NamaLengkap)
                .Select(u => new {
                    u.Id,
                    u.NamaLengkap,
                    u.Email,
                    u.RoleId,
                    RoleName = u.Role != null ? u.Role.NamaRole : "-",
                    u.IsActive,
                    CreatedAt = u.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                });

            return Ok(result);
        }

        [HttpPost]
        [HttpPost("/api/admin/CreateUser")]
        public async Task<IActionResult> Create([FromForm] CreateUserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.NamaLengkap))
                return Ok(new { success = false, message = "Nama lengkap wajib diisi." });
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains('@'))
                return Ok(new { success = false, message = "Email tidak valid." });
            if (string.IsNullOrWhiteSpace(model.PasswordHash) || model.PasswordHash.Length < 6)
                return Ok(new { success = false, message = "Password minimal 6 karakter." });
            if (!string.IsNullOrEmpty(model.ConfirmPassword) && model.ConfirmPassword != model.PasswordHash)
                return Ok(new { success = false, message = "Password dan konfirmasi tidak cocok." });
            if (model.RoleId <= 0)
                return Ok(new { success = false, message = "Pilih role terlebih dahulu." });

            var existing = await _userRepo.GetByEmailAsync(model.Email);
            if (existing != null)
                return Ok(new { success = false, message = "Email sudah terdaftar." });

            var user = new User
            {
                NamaLengkap = model.NamaLengkap.Trim(),
                Email = model.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash),
                RoleId = model.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Berhasil menambah user baru." });
        }

        [HttpPost("{id}")]
        [HttpPut("{id}")]
        [HttpPost("/api/admin/EditUser/{id?}")]
        public async Task<IActionResult> Edit([FromRoute] int? id, [FromForm] EditUserDto model)
        {
            var targetId = id.HasValue && id.Value > 0 ? id.Value : model.Id;

            if (targetId <= 0)
                return Ok(new { success = false, message = "ID tidak valid." });

            var current = await _userRepo.GetByIdAsync(targetId);
            if (current == null)
                return Ok(new { success = false, message = "User tidak ditemukan." });

            if (string.IsNullOrWhiteSpace(model.NamaLengkap))
                return Ok(new { success = false, message = "Nama lengkap wajib diisi." });
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains('@'))
                return Ok(new { success = false, message = "Email tidak valid." });

            var emailConflict = await _userRepo.GetByEmailAsync(model.Email);
            if (emailConflict != null && emailConflict.Id != targetId)
                return Ok(new { success = false, message = "Email sudah digunakan user lain." });

            current.NamaLengkap = model.NamaLengkap.Trim();
            current.Email = model.Email.Trim();
            current.RoleId = model.RoleId;
            current.IsActive = model.IsActive;
            current.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password.Length >= 6)
                current.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            await _userRepo.UpdateAsync(current);
            await _userRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Berhasil memperbarui data user." });
        }

        [HttpPost("delete")]
        [HttpDelete("{id}")]
        [HttpPost("/api/admin/DeleteUser/{id?}")]
        public async Task<IActionResult> Delete([FromRoute] int? id, [FromForm] DeleteRequestDto? formDto)
        {
            var targetId = id.HasValue && id.Value > 0 ? id.Value : (formDto?.Id ?? 0);
            if (targetId <= 0)
                return Ok(new { success = false, message = "ID tidak valid." });

            var user = await _userRepo.GetByIdAsync(targetId);
            if (user == null)
                return Ok(new { success = false, message = "User tidak ditemukan." });

            await _userRepo.DeleteAsync(targetId);
            await _userRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Berhasil menghapus user." });
        }
    }
}
