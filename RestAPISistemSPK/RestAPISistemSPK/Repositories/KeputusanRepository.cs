using Microsoft.EntityFrameworkCore;
using RestAPISistemSPK.Data;
using RestAPISistemSPK.Interfaces;
using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Repositories
{
    public class KeputusanRepository : IKeputusanRepository
    {
        private readonly AppDbContext _context;

        // Saaty's Random Index (RI) table for matrix sizes 1..15
        private static readonly Dictionary<int, double> RI_TABLE = new()
        {
            { 1, 0.00 }, { 2, 0.00 }, { 3, 0.58 }, { 4, 0.90 }, { 5, 1.12 },
            { 6, 1.24 }, { 7, 1.32 }, { 8, 1.41 }, { 9, 1.45 }, { 10, 1.49 },
            { 11, 1.51 }, { 12, 1.54 }, { 13, 1.56 }, { 14, 1.57 }, { 15, 1.59 }
        };

        public KeputusanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CalculateKeputusanResponseDto> CalculateAsync(CalculateKeputusanRequestDto request)
        {
            var response = new CalculateKeputusanResponseDto();

            if (request.ParameterIds == null || request.ParameterIds.Count == 0 ||
                request.SupplierIds == null || request.SupplierIds.Count == 0)
            {
                return response;
            }

            var parameters = await _context.Parameters
                .AsNoTracking()
                .Where(p => request.ParameterIds.Contains(p.Id))
                .ToListAsync();

            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Where(s => request.SupplierIds.Contains(s.Id))
                .ToListAsync();

            int n = parameters.Count;
            if (n == 0 || suppliers.Count == 0) return response;

            // 1. Construct Pairwise Matrix AHP
            double[,] matrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) matrix[i, j] = 1.0;
                    else matrix[i, j] = 1.0; // default
                }
            }

            // Fill provided comparisons
            if (request.Comparisons != null && request.Comparisons.Count > 0)
            {
                foreach (var cmp in request.Comparisons)
                {
                    int i = parameters.FindIndex(p => p.Id == cmp.Param1Id);
                    int j = parameters.FindIndex(p => p.Id == cmp.Param2Id);
                    if (i >= 0 && j >= 0 && i != j && cmp.Value > 0)
                    {
                        matrix[i, j] = cmp.Value;
                        matrix[j, i] = 1.0 / cmp.Value;
                    }
                }
            }

            // 2. Column Sums
            double[] colSums = new double[n];
            for (int j = 0; j < n; j++)
            {
                double sum = 0;
                for (int i = 0; i < n; i++) sum += matrix[i, j];
                colSums[j] = sum;
            }

            // 3. Normalized Matrix & Priority Weights
            double[] weights = new double[n];
            if (request.DirectWeights != null && request.DirectWeights.Count > 0)
            {
                double totalDirect = 0;
                for (int i = 0; i < n; i++)
                {
                    double w = 0;
                    if (request.DirectWeights.TryGetValue(parameters[i].Id, out double val) && val > 0)
                    {
                        w = val;
                    }
                    weights[i] = w;
                    totalDirect += w;
                }

                if (totalDirect > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        weights[i] /= totalDirect;
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++) weights[i] = 1.0 / n;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    double rowSum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        rowSum += (colSums[j] > 0 ? (matrix[i, j] / colSums[j]) : 0);
                    }
                    weights[i] = rowSum / n;
                }
            }

            // 4. Lambda Max, CI, CR
            double lambdaMax = 0;
            for (int i = 0; i < n; i++)
            {
                double rowWeightedSum = 0;
                for (int j = 0; j < n; j++)
                {
                    rowWeightedSum += matrix[i, j] * weights[j];
                }
                lambdaMax += (weights[i] > 0 ? (rowWeightedSum / weights[i]) : 0);
            }
            lambdaMax = n > 0 ? (lambdaMax / n) : n;

            double ci = n > 1 ? ((lambdaMax - n) / (n - 1)) : 0;
            double ri = RI_TABLE.ContainsKey(n) ? RI_TABLE[n] : 1.59;
            double cr = ri > 0 ? (ci / ri) : 0;
            if (cr < 0) cr = 0;

            bool isConsistent = (n <= 2) || (cr <= 0.1001);

            response.IsConsistent = isConsistent;
            response.ConsistencyRatio = Math.Round(cr, 4);
            response.LambdaMax = Math.Round(lambdaMax, 4);
            response.ConsistencyIndex = Math.Round(ci, 4);
            response.RandomIndex = ri;

            // Map Parameter Weights
            for (int i = 0; i < n; i++)
            {
                var p = parameters[i];
                response.ParameterWeights.Add(new ParameterWeightResultDto
                {
                    ParameterId = p.Id,
                    KodeParameter = p.KodeParameter,
                    NamaParameter = p.NamaParameter,
                    SifatKriteria = p.SifatKriteria,
                    Bobot = Math.Round(weights[i], 4),
                    Persentase = Math.Round(weights[i] * 100, 2)
                });
            }

            // 5. Evaluate Supplier Real Values & Normalization
            // Build lookup of input values: SupplierId -> (ParameterId -> NilaiRiil)
            var inputDict = new Dictionary<Guid, Dictionary<Guid, decimal>>();
            foreach (var sup in suppliers)
            {
                inputDict[sup.Id] = new Dictionary<Guid, decimal>();
                foreach (var param in parameters)
                {
                    inputDict[sup.Id][param.Id] = 0;
                }
            }

            if (request.NilaiInputs != null)
            {
                foreach (var inp in request.NilaiInputs)
                {
                    if (inputDict.ContainsKey(inp.SupplierId))
                    {
                        inputDict[inp.SupplierId][inp.ParameterId] = inp.NilaiRiil;
                    }
                }
            }

            // Min and Max per parameter across all suppliers
            var minVals = new Dictionary<Guid, decimal>();
            var maxVals = new Dictionary<Guid, decimal>();
            foreach (var param in parameters)
            {
                var values = suppliers.Select(s => inputDict[s.Id][param.Id]).ToList();
                minVals[param.Id] = values.Count > 0 ? values.Min() : 0;
                maxVals[param.Id] = values.Count > 0 ? values.Max() : 0;
            }

            // Normalized scores per supplier per parameter
            var normScores = new Dictionary<Guid, Dictionary<Guid, decimal>>();
            var compositeScores = new Dictionary<Guid, decimal>();

            for (int sIdx = 0; sIdx < suppliers.Count; sIdx++)
            {
                var sup = suppliers[sIdx];
                normScores[sup.Id] = new Dictionary<Guid, decimal>();
                decimal totalScore = 0;

                for (int pIdx = 0; pIdx < n; pIdx++)
                {
                    var param = parameters[pIdx];
                    decimal val = inputDict[sup.Id][param.Id];
                    decimal norm = 0;

                    if (param.SifatKriteria.Equals("Cost", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cost: Min / Val (Semakin kecil semakin baik)
                        decimal min = minVals[param.Id];
                        if (val > 0)
                        {
                            norm = min / val;
                        }
                        else
                        {
                            norm = (min == 0) ? 1.0m : 0m;
                        }
                    }
                    else
                    {
                        // Benefit: Val / Max (Semakin besar semakin baik)
                        decimal max = maxVals[param.Id];
                        if (max > 0)
                        {
                            norm = val / max;
                        }
                        else
                        {
                            norm = 1.0m;
                        }
                    }

                    if (norm > 1.0m) norm = 1.0m;
                    if (norm < 0m) norm = 0m;

                    normScores[sup.Id][param.Id] = Math.Round(norm, 4);
                    totalScore += norm * (decimal)weights[pIdx];
                }

                compositeScores[sup.Id] = Math.Round(totalScore, 4);
            }

            // 6. Rank Suppliers
            var sortedSuppliers = suppliers
                .OrderByDescending(s => compositeScores[s.Id])
                .ToList();

            int rank = 1;
            for (int i = 0; i < sortedSuppliers.Count; i++)
            {
                var sup = sortedSuppliers[i];
                var score = compositeScores[sup.Id];

                string statusRekomendasi = "Pilihan Utama";
                if (rank == 2) statusRekomendasi = "Alternatif Cadangan";
                else if (rank >= 3)
                {
                    statusRekomendasi = score >= 0.60m ? "Pertimbangan Khusus" : "Tidak Direkomendasikan";
                }

                var rankDto = new CalculatedSupplierRankingDto
                {
                    Peringkat = rank,
                    SupplierId = sup.Id,
                    KodeSupplier = sup.KodeSupplier,
                    NamaSupplier = sup.NamaSupplier,
                    SkorAkhir = score,
                    StatusRekomendasi = statusRekomendasi,
                    RealValues = inputDict[sup.Id],
                    NormalizedScores = normScores[sup.Id]
                };

                response.Rankings.Add(rankDto);
                rank++;
            }

            // Winner Highlight
            if (response.Rankings.Count > 0)
            {
                var winner = response.Rankings[0];
                var topParams = response.ParameterWeights
                    .OrderByDescending(pw => pw.Bobot)
                    .Take(2)
                    .Select(pw => pw.NamaParameter)
                    .ToList();

                string paramHighlights = string.Join(" dan ", topParams);
                response.WinnerHighlight = $"Supplier {winner.NamaSupplier} unggul di peringkat 1 dengan skor kelayakan {winner.SkorAkhir:N4}, memiliki keunggulan kompetitif pada aspek {paramHighlights}.";
            }

            return response;
        }

        public async Task<Keputusan> SaveAsync(SaveKeputusanRequestDto dto)
        {
            Keputusan? keputusan = null;

            if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
            {
                keputusan = await _context.Keputusans
                    .Include(k => k.KeputusanDetails)
                        .ThenInclude(kd => kd.KeputusanNilais)
                    .FirstOrDefaultAsync(k => k.Id == dto.Id.Value);
            }

            var kode = dto.KodeKeputusan;
            if (string.IsNullOrWhiteSpace(kode))
            {
                if (keputusan != null && !string.IsNullOrWhiteSpace(keputusan.KodeKeputusan))
                {
                    kode = keputusan.KodeKeputusan;
                }
                else
                {
                    var count = await _context.Keputusans.CountAsync() + 1;
                    kode = $"KPTS-{DateTime.UtcNow:yyyyMMdd}-{count:D3}";
                }
            }

            var status = !string.IsNullOrWhiteSpace(dto.Status)
                ? dto.Status
                : (dto.SupplierPemenangId.HasValue ? "Selesai" : "Draft");

            if (keputusan == null)
            {
                keputusan = new Keputusan
                {
                    Id = (dto.Id.HasValue && dto.Id.Value != Guid.Empty) ? dto.Id.Value : Guid.NewGuid(),
                    KodeKeputusan = kode.Trim(),
                    NamaKeputusan = dto.NamaKeputusan.Trim(),
                    TanggalKeputusan = dto.TanggalKeputusan,
                    AspekId = dto.AspekId,
                    Keterangan = dto.Keterangan?.Trim(),
                    SupplierPemenangId = dto.SupplierPemenangId,
                    SkorTertinggi = dto.SkorTertinggi,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Keputusans.AddAsync(keputusan);
            }
            else
            {
                keputusan.NamaKeputusan = dto.NamaKeputusan.Trim();
                keputusan.TanggalKeputusan = dto.TanggalKeputusan;
                keputusan.AspekId = dto.AspekId;
                keputusan.Keterangan = dto.Keterangan?.Trim();
                keputusan.SupplierPemenangId = dto.SupplierPemenangId;
                keputusan.SkorTertinggi = dto.SkorTertinggi;
                keputusan.Status = status;
                keputusan.UpdatedAt = DateTime.UtcNow;

                // Remove existing details and nilais to allow clean upsert
                if (keputusan.KeputusanDetails != null && keputusan.KeputusanDetails.Count > 0)
                {
                    foreach (var oldDetail in keputusan.KeputusanDetails)
                    {
                        if (oldDetail.KeputusanNilais != null && oldDetail.KeputusanNilais.Count > 0)
                        {
                            _context.KeputusanNilais.RemoveRange(oldDetail.KeputusanNilais);
                        }
                    }
                    _context.KeputusanDetails.RemoveRange(keputusan.KeputusanDetails);
                }
            }

            if (dto.Details != null && dto.Details.Count > 0)
            {
                foreach (var d in dto.Details)
                {
                    Guid supplierId = Guid.Empty;
                    if (d.SupplierId.HasValue && d.SupplierId.Value != Guid.Empty)
                    {
                        supplierId = d.SupplierId.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(d.NamaSupplier))
                    {
                        var trimmedName = d.NamaSupplier.Trim();
                        var existingSup = await _context.Suppliers.FirstOrDefaultAsync(s => s.NamaSupplier.ToLower() == trimmedName.ToLower());
                        if (existingSup != null)
                        {
                            supplierId = existingSup.Id;
                        }
                        else
                        {
                            var supCount = await _context.Suppliers.CountAsync() + 1;
                            var newSup = new Supplier
                            {
                                Id = Guid.NewGuid(),
                                KodeSupplier = $"SPL-{supCount:D3}",
                                NamaSupplier = trimmedName,
                                Status = "Aktif",
                                CreatedAt = DateTime.UtcNow
                            };
                            await _context.Suppliers.AddAsync(newSup);
                            supplierId = newSup.Id;
                        }
                    }

                    if (supplierId == Guid.Empty) continue;

                    var detail = new KeputusanDetail
                    {
                        Id = Guid.NewGuid(),
                        KeputusanId = keputusan.Id,
                        SupplierId = supplierId,
                        SkorAkhir = d.SkorAkhir,
                        Peringkat = d.Peringkat,
                        StatusRekomendasi = d.StatusRekomendasi ?? (d.Peringkat.HasValue ? "Pilihan" : "Draft"),
                        CatatanEvaluasi = d.CatatanEvaluasi,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.KeputusanDetails.AddAsync(detail);

                    if (d.Nilais != null && d.Nilais.Count > 0)
                    {
                        foreach (var n in d.Nilais)
                        {
                            var nilai = new KeputusanNilai
                            {
                                Id = Guid.NewGuid(),
                                KeputusanDetailId = detail.Id,
                                ParameterId = n.ParameterId,
                                NilaiRiil = n.NilaiRiil,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _context.KeputusanNilais.AddAsync(nilai);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return keputusan;
        }

        public async Task<List<KeputusanListDto>> GetAllAsync(string? status = null)
        {
            var query = _context.Keputusans
                .AsNoTracking()
                .Include(k => k.Aspek)
                .Include(k => k.SupplierPemenang)
                .Include(k => k.KeputusanDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(k => k.Status == status);
            }

            return await query
                .OrderByDescending(k => k.TanggalKeputusan)
                .ThenByDescending(k => k.CreatedAt)
                .Select(k => new KeputusanListDto
                {
                    Id = k.Id,
                    KodeKeputusan = k.KodeKeputusan,
                    NamaKeputusan = k.NamaKeputusan,
                    TanggalKeputusan = k.TanggalKeputusan,
                    AspekId = k.AspekId,
                    NamaAspek = k.Aspek != null ? k.Aspek.NamaAspek : "-",
                    Keterangan = k.Keterangan,
                    SupplierPemenangId = k.SupplierPemenangId,
                    NamaSupplierPemenang = k.SupplierPemenang != null ? k.SupplierPemenang.NamaSupplier : "-",
                    KodeSupplierPemenang = k.SupplierPemenang != null ? k.SupplierPemenang.KodeSupplier : "-",
                    SkorTertinggi = k.SkorTertinggi,
                    Status = k.Status,
                    JumlahSupplier = k.KeputusanDetails.Count,
                    CreatedAt = k.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<KeputusanDetailResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Keputusans
                .AsNoTracking()
                .Include(k => k.Aspek)
                .Include(k => k.SupplierPemenang)
                .Include(k => k.KeputusanDetails)
                    .ThenInclude(kd => kd.Supplier)
                .Include(k => k.KeputusanDetails)
                    .ThenInclude(kd => kd.KeputusanNilais)
                        .ThenInclude(kn => kn.Parameter)
                .Where(k => k.Id == id)
                .Select(k => new KeputusanDetailResponseDto
                {
                    Id = k.Id,
                    KodeKeputusan = k.KodeKeputusan,
                    NamaKeputusan = k.NamaKeputusan,
                    TanggalKeputusan = k.TanggalKeputusan,
                    AspekId = k.AspekId,
                    NamaAspek = k.Aspek != null ? k.Aspek.NamaAspek : "-",
                    Keterangan = k.Keterangan,
                    SupplierPemenangId = k.SupplierPemenangId,
                    NamaSupplierPemenang = k.SupplierPemenang != null ? k.SupplierPemenang.NamaSupplier : "-",
                    SkorTertinggi = k.SkorTertinggi,
                    Status = k.Status,
                    CreatedAt = k.CreatedAt,
                    Details = k.KeputusanDetails
                        .OrderBy(kd => kd.Peringkat ?? 999)
                        .ThenBy(kd => kd.CreatedAt)
                        .Select(kd => new KeputusanDetailItemDto
                        {
                            Id = kd.Id,
                            SupplierId = kd.SupplierId,
                            KodeSupplier = kd.Supplier != null ? kd.Supplier.KodeSupplier : "-",
                            NamaSupplier = kd.Supplier != null ? kd.Supplier.NamaSupplier : "-",
                            SkorAkhir = kd.SkorAkhir,
                            Peringkat = kd.Peringkat,
                            StatusRekomendasi = kd.StatusRekomendasi,
                            CatatanEvaluasi = kd.CatatanEvaluasi,
                            Nilais = kd.KeputusanNilais.Select(kn => new KeputusanNilaiItemDto
                            {
                                Id = kn.Id,
                                ParameterId = kn.ParameterId,
                                KodeParameter = kn.Parameter != null ? kn.Parameter.KodeParameter : "-",
                                NamaParameter = kn.Parameter != null ? kn.Parameter.NamaParameter : "-",
                                Satuan = kn.Parameter != null ? kn.Parameter.Satuan : "-",
                                SifatKriteria = kn.Parameter != null ? kn.Parameter.SifatKriteria : "Cost",
                                NilaiRiil = kn.NilaiRiil,
                                Bobot = kn.Parameter != null ? kn.Parameter.Bobot : 0
                            }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var keputusan = await _context.Keputusans
                .Include(k => k.KeputusanDetails)
                    .ThenInclude(kd => kd.KeputusanNilais)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (keputusan == null) return false;

            _context.Keputusans.Remove(keputusan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var totalSupplier = await _context.Suppliers.CountAsync();
            var totalAspek = await _context.Aspeks.CountAsync();
            var totalParameter = await _context.Parameters.CountAsync();
            var totalKeputusan = await _context.Keputusans.CountAsync(k => k.Status == "Selesai");

            var latest = await _context.Keputusans
                .AsNoTracking()
                .Where(k => k.Status == "Selesai")
                .Include(k => k.SupplierPemenang)
                .Include(k => k.KeputusanDetails)
                .OrderByDescending(k => k.TanggalKeputusan)
                .ThenByDescending(k => k.CreatedAt)
                .Select(k => new KeputusanListDto
                {
                    Id = k.Id,
                    KodeKeputusan = k.KodeKeputusan,
                    NamaKeputusan = k.NamaKeputusan,
                    TanggalKeputusan = k.TanggalKeputusan,
                    Keterangan = k.Keterangan,
                    SupplierPemenangId = k.SupplierPemenangId,
                    NamaSupplierPemenang = k.SupplierPemenang != null ? k.SupplierPemenang.NamaSupplier : "-",
                    KodeSupplierPemenang = k.SupplierPemenang != null ? k.SupplierPemenang.KodeSupplier : "-",
                    SkorTertinggi = k.SkorTertinggi,
                    Status = k.Status,
                    JumlahSupplier = k.KeputusanDetails.Count,
                    CreatedAt = k.CreatedAt
                })
                .FirstOrDefaultAsync();

            var topSuppliers = await _context.Suppliers
                .AsNoTracking()
                .OrderByDescending(s => s.KeputusanDetails.Count(kd => kd.Peringkat == 1))
                .ThenByDescending(s => s.CreatedAt)
                .Take(5)
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

            return new DashboardSummaryDto
            {
                TotalSupplier = totalSupplier,
                TotalAspek = totalAspek,
                TotalParameter = totalParameter,
                TotalKeputusan = totalKeputusan,
                KeputusanTerakhir = latest,
                TopSuppliers = topSuppliers
            };
        }
    }
}
