using Microsoft.EntityFrameworkCore;
using RestAPISistemSPK.Data;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    KodeSupplier = s.KodeSupplier,
                    NamaSupplier = s.NamaSupplier,
                    Email = s.Email,
                    Alamat = s.Alamat,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    TotalEvaluasiDiikuti = s.KeputusanDetails.Count
                })
                .ToListAsync();
        }

        public async Task<SupplierDto?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    KodeSupplier = s.KodeSupplier,
                    NamaSupplier = s.NamaSupplier,
                    Email = s.Email,
                    Alamat = s.Alamat,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    TotalEvaluasiDiikuti = s.KeputusanDetails.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Supplier> CreateAsync(CreateSupplierDto dto)
        {
            var kode = dto.KodeSupplier;
            if (string.IsNullOrWhiteSpace(kode))
            {
                var count = await _context.Suppliers.CountAsync() + 1;
                kode = $"SPL-{DateTime.UtcNow:yyyyMMdd}-{count:D3}";
            }

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                KodeSupplier = kode.Trim(),
                NamaSupplier = dto.NamaSupplier.Trim(),
                Email = dto.Email?.Trim(),
                Alamat = dto.Alamat?.Trim(),
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Aktif" : dto.Status.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> UpdateAsync(UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(dto.Id);
            if (supplier == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.KodeSupplier))
                supplier.KodeSupplier = dto.KodeSupplier.Trim();

            supplier.NamaSupplier = dto.NamaSupplier.Trim();
            supplier.Email = dto.Email?.Trim();
            supplier.Alamat = dto.Alamat?.Trim();
            supplier.Status = string.IsNullOrWhiteSpace(dto.Status) ? "Aktif" : dto.Status.Trim();
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            // Check if supplier has decision history
            var hasDecisions = await _context.KeputusanDetails.AnyAsync(kd => kd.SupplierId == id);
            if (hasDecisions)
            {
                // Soft delete by marking inactive to preserve audit history
                supplier.Status = "Tidak Aktif";
                supplier.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }
    }
}
