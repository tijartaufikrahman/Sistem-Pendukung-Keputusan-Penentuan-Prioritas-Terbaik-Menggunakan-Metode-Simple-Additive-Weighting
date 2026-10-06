using Microsoft.EntityFrameworkCore;
using RestAPISistemSPK.Data;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Repositories
{
    public class KriteriaRepository : IKriteriaRepository
    {
        private readonly AppDbContext _context;

        public KriteriaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ================= ASPEK =================
        public async Task<List<AspekDto>> GetAllAspekAsync()
        {
            return await _context.Aspeks
                .AsNoTracking()
                .OrderBy(a => a.Urutan)
                .Select(a => new AspekDto
                {
                    Id = a.Id,
                    KodeAspek = a.KodeAspek,
                    NamaAspek = a.NamaAspek,
                    Deskripsi = a.Deskripsi,
                    Urutan = a.Urutan,
                    JumlahParameter = a.Parameters.Count,
                    CreatedAt = a.CreatedAt,
                    Parameters = a.Parameters.OrderBy(p => p.KodeParameter).Select(p => new ParameterDto
                    {
                        Id = p.Id,
                        AspekId = p.AspekId,
                        NamaAspek = a.NamaAspek,
                        KodeParameter = p.KodeParameter,
                        NamaParameter = p.NamaParameter,
                        Satuan = p.Satuan,
                        SifatKriteria = p.SifatKriteria,
                        Bobot = p.Bobot,
                        Keterangan = p.Keterangan,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<AspekDto?> GetAspekByIdAsync(Guid id)
        {
            return await _context.Aspeks
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AspekDto
                {
                    Id = a.Id,
                    KodeAspek = a.KodeAspek,
                    NamaAspek = a.NamaAspek,
                    Deskripsi = a.Deskripsi,
                    Urutan = a.Urutan,
                    JumlahParameter = a.Parameters.Count,
                    CreatedAt = a.CreatedAt,
                    Parameters = a.Parameters.OrderBy(p => p.KodeParameter).Select(p => new ParameterDto
                    {
                        Id = p.Id,
                        AspekId = p.AspekId,
                        NamaAspek = a.NamaAspek,
                        KodeParameter = p.KodeParameter,
                        NamaParameter = p.NamaParameter,
                        Satuan = p.Satuan,
                        SifatKriteria = p.SifatKriteria,
                        Bobot = p.Bobot,
                        Keterangan = p.Keterangan,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Aspek> CreateAspekAsync(CreateAspekDto dto)
        {
            var kode = dto.KodeAspek;
            if (string.IsNullOrWhiteSpace(kode))
            {
                var count = await _context.Aspeks.CountAsync() + 1;
                kode = $"ASP-{count:D2}";
            }

            var aspek = new Aspek
            {
                Id = Guid.NewGuid(),
                KodeAspek = kode.Trim(),
                NamaAspek = dto.NamaAspek.Trim(),
                Deskripsi = dto.Deskripsi?.Trim(),
                Urutan = dto.Urutan,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Aspeks.AddAsync(aspek);
            await _context.SaveChangesAsync();
            return aspek;
        }

        public async Task<Aspek?> UpdateAspekAsync(UpdateAspekDto dto)
        {
            var aspek = await _context.Aspeks.FindAsync(dto.Id);
            if (aspek == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.KodeAspek))
                aspek.KodeAspek = dto.KodeAspek.Trim();

            aspek.NamaAspek = dto.NamaAspek.Trim();
            aspek.Deskripsi = dto.Deskripsi?.Trim();
            aspek.Urutan = dto.Urutan;
            aspek.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return aspek;
        }

        public async Task<bool> DeleteAspekAsync(Guid id)
        {
            var aspek = await _context.Aspeks.Include(a => a.Parameters).FirstOrDefaultAsync(a => a.Id == id);
            if (aspek == null) return false;

            _context.Aspeks.Remove(aspek);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAspekAsync()
        {
            return await _context.Aspeks.CountAsync();
        }

        // ================= PARAMETER =================
        public async Task<List<ParameterDto>> GetAllParameterAsync(Guid? aspekId = null)
        {
            var query = _context.Parameters
                .AsNoTracking()
                .Include(p => p.Aspek)
                .AsQueryable();

            if (aspekId.HasValue && aspekId.Value != Guid.Empty)
            {
                query = query.Where(p => p.AspekId == aspekId.Value);
            }

            return await query
                .OrderBy(p => p.Aspek != null ? p.Aspek.Urutan : 0)
                .ThenBy(p => p.KodeParameter)
                .Select(p => new ParameterDto
                {
                    Id = p.Id,
                    AspekId = p.AspekId,
                    NamaAspek = p.Aspek != null ? p.Aspek.NamaAspek : "-",
                    KodeParameter = p.KodeParameter,
                    NamaParameter = p.NamaParameter,
                    Satuan = p.Satuan,
                    SifatKriteria = p.SifatKriteria,
                    Bobot = p.Bobot,
                    Keterangan = p.Keterangan,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ParameterDto?> GetParameterByIdAsync(Guid id)
        {
            return await _context.Parameters
                .AsNoTracking()
                .Include(p => p.Aspek)
                .Where(p => p.Id == id)
                .Select(p => new ParameterDto
                {
                    Id = p.Id,
                    AspekId = p.AspekId,
                    NamaAspek = p.Aspek != null ? p.Aspek.NamaAspek : "-",
                    KodeParameter = p.KodeParameter,
                    NamaParameter = p.NamaParameter,
                    Satuan = p.Satuan,
                    SifatKriteria = p.SifatKriteria,
                    Bobot = p.Bobot,
                    Keterangan = p.Keterangan,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Parameter> CreateParameterAsync(CreateParameterDto dto)
        {
            var kode = dto.KodeParameter;
            if (string.IsNullOrWhiteSpace(kode))
            {
                var count = await _context.Parameters.CountAsync() + 1;
                kode = $"PRM-{count:D2}";
                while (await _context.Parameters.AnyAsync(p => p.KodeParameter == kode))
                {
                    count++;
                    kode = $"PRM-{count:D2}";
                }
            }

            var parameter = new Parameter
            {
                Id = Guid.NewGuid(),
                AspekId = dto.AspekId,
                KodeParameter = kode.Trim(),
                NamaParameter = dto.NamaParameter.Trim(),
                Satuan = dto.Satuan.Trim(),
                SifatKriteria = string.IsNullOrWhiteSpace(dto.SifatKriteria) ? "Cost" : dto.SifatKriteria.Trim(),
                Bobot = dto.Bobot,
                Keterangan = dto.Keterangan?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Parameters.AddAsync(parameter);
            await _context.SaveChangesAsync();
            return parameter;
        }

        public async Task<Parameter?> UpdateParameterAsync(UpdateParameterDto dto)
        {
            var parameter = await _context.Parameters.FindAsync(dto.Id);
            if (parameter == null) return null;

            parameter.AspekId = dto.AspekId;
            if (!string.IsNullOrWhiteSpace(dto.KodeParameter))
                parameter.KodeParameter = dto.KodeParameter.Trim();

            parameter.NamaParameter = dto.NamaParameter.Trim();
            parameter.Satuan = dto.Satuan.Trim();
            parameter.SifatKriteria = string.IsNullOrWhiteSpace(dto.SifatKriteria) ? "Cost" : dto.SifatKriteria.Trim();
            parameter.Bobot = dto.Bobot;
            parameter.Keterangan = dto.Keterangan?.Trim();
            parameter.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return parameter;
        }

        public async Task<bool> UpdateBatchBobotAsync(List<ParameterBobotItemDto> items)
        {
            if (items == null || items.Count == 0) return false;

            var ids = items.Select(i => i.Id).ToList();
            var parameters = await _context.Parameters.Where(p => ids.Contains(p.Id)).ToListAsync();

            foreach (var p in parameters)
            {
                var item = items.FirstOrDefault(i => i.Id == p.Id);
                if (item != null)
                {
                    p.Bobot = item.Bobot;
                    p.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteParameterAsync(Guid id)
        {
            var parameter = await _context.Parameters.FindAsync(id);
            if (parameter == null) return false;

            _context.Parameters.Remove(parameter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountParameterAsync()
        {
            return await _context.Parameters.CountAsync();
        }
    }
}
