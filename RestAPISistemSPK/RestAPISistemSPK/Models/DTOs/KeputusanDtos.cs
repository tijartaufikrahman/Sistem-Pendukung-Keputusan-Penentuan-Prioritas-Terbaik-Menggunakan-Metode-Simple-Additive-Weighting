namespace RestAPISistemSPK.Models.DTOs
{
    // INPUT PERHITUNGAN AHP & EVALUASI
    public class PairwiseComparisonDto
    {
        public Guid Param1Id { get; set; }
        public Guid Param2Id { get; set; }
        public double Value { get; set; } = 1.0; // 1 to 9, or reciprocal 1/9 to 1
    }

    public class SupplierNilaiInputDto
    {
        public Guid SupplierId { get; set; }
        public Guid ParameterId { get; set; }
        public decimal NilaiRiil { get; set; }
    }

    public class CalculateKeputusanRequestDto
    {
        public string? NamaKeputusan { get; set; }
        public List<Guid> SupplierIds { get; set; } = new();
        public List<Guid> ParameterIds { get; set; } = new();
        public List<SupplierNilaiInputDto> NilaiInputs { get; set; } = new();
        public List<PairwiseComparisonDto> Comparisons { get; set; } = new();
        public Dictionary<Guid, double>? DirectWeights { get; set; } // Key: ParameterId, Value: Bobot (0.0 - 1.0 atau persen 0 - 100)
    }

    // OUTPUT PERHITUNGAN AHP & EVALUASI
    public class ParameterWeightResultDto
    {
        public Guid ParameterId { get; set; }
        public string KodeParameter { get; set; } = string.Empty;
        public string NamaParameter { get; set; } = string.Empty;
        public string SifatKriteria { get; set; } = "Cost";
        public double Bobot { get; set; }
        public double Persentase { get; set; }
    }

    public class CalculatedSupplierRankingDto
    {
        public int Peringkat { get; set; }
        public Guid SupplierId { get; set; }
        public string KodeSupplier { get; set; } = string.Empty;
        public string NamaSupplier { get; set; } = string.Empty;
        public decimal SkorAkhir { get; set; }
        public string StatusRekomendasi { get; set; } = "Pilihan Utama";
        public string AlasanKemenangan { get; set; } = string.Empty;
        public Dictionary<Guid, decimal> RealValues { get; set; } = new();
        public Dictionary<Guid, decimal> NormalizedScores { get; set; } = new();
    }

    public class CalculateKeputusanResponseDto
    {
        public bool IsConsistent { get; set; }
        public double ConsistencyRatio { get; set; }
        public double LambdaMax { get; set; }
        public double ConsistencyIndex { get; set; }
        public double RandomIndex { get; set; }
        public List<ParameterWeightResultDto> ParameterWeights { get; set; } = new();
        public List<CalculatedSupplierRankingDto> Rankings { get; set; } = new();
        public string WinnerHighlight { get; set; } = string.Empty;
    }

    // SIMPAN TRANSAKSI KEPUTUSAN
    public class SaveKeputusanNilaiDto
    {
        public Guid ParameterId { get; set; }
        public decimal NilaiRiil { get; set; }
    }

    public class SaveKeputusanDetailDto
    {
        public Guid? SupplierId { get; set; }
        public string? NamaSupplier { get; set; }
        public decimal? SkorAkhir { get; set; }
        public int? Peringkat { get; set; }
        public string? StatusRekomendasi { get; set; } = "Draft";
        public string? CatatanEvaluasi { get; set; }
        public List<SaveKeputusanNilaiDto> Nilais { get; set; } = new();
    }

    public class SaveKeputusanRequestDto
    {
        public Guid? Id { get; set; }
        public string? KodeKeputusan { get; set; }
        public string NamaKeputusan { get; set; } = string.Empty;
        public DateTime TanggalKeputusan { get; set; } = DateTime.UtcNow;
        public Guid? AspekId { get; set; }
        public string? Keterangan { get; set; }
        public Guid? SupplierPemenangId { get; set; }
        public decimal? SkorTertinggi { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Selesai
        public List<SaveKeputusanDetailDto> Details { get; set; } = new();
    }

    // DAFTAR & DETAIL KEPUTUSAN UNTUK RIWAYAT
    public class KeputusanListDto
    {
        public Guid Id { get; set; }
        public string KodeKeputusan { get; set; } = string.Empty;
        public string NamaKeputusan { get; set; } = string.Empty;
        public DateTime TanggalKeputusan { get; set; }
        public Guid? AspekId { get; set; }
        public string? NamaAspek { get; set; }
        public string? Keterangan { get; set; }
        public Guid? SupplierPemenangId { get; set; }
        public string? NamaSupplierPemenang { get; set; }
        public string? KodeSupplierPemenang { get; set; }
        public decimal? SkorTertinggi { get; set; }
        public string Status { get; set; } = "Selesai";
        public int JumlahSupplier { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KeputusanDetailItemDto
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string KodeSupplier { get; set; } = string.Empty;
        public string NamaSupplier { get; set; } = string.Empty;
        public decimal? SkorAkhir { get; set; }
        public int? Peringkat { get; set; }
        public string? StatusRekomendasi { get; set; } = string.Empty;
        public string? CatatanEvaluasi { get; set; }
        public List<KeputusanNilaiItemDto> Nilais { get; set; } = new();
    }

    public class KeputusanNilaiItemDto
    {
        public Guid Id { get; set; }
        public Guid ParameterId { get; set; }
        public string KodeParameter { get; set; } = string.Empty;
        public string NamaParameter { get; set; } = string.Empty;
        public string Satuan { get; set; } = string.Empty;
        public string SifatKriteria { get; set; } = "Cost";
        public decimal NilaiRiil { get; set; }
        public decimal Bobot { get; set; }
    }

    public class KeputusanDetailResponseDto
    {
        public Guid Id { get; set; }
        public string KodeKeputusan { get; set; } = string.Empty;
        public string NamaKeputusan { get; set; } = string.Empty;
        public DateTime TanggalKeputusan { get; set; }
        public Guid? AspekId { get; set; }
        public string? NamaAspek { get; set; }
        public string? Keterangan { get; set; }
        public Guid? SupplierPemenangId { get; set; }
        public string? NamaSupplierPemenang { get; set; }
        public decimal? SkorTertinggi { get; set; }
        public string Status { get; set; } = "Selesai";
        public DateTime CreatedAt { get; set; }
        public List<KeputusanDetailItemDto> Details { get; set; } = new();
    }

    // DASHBOARD
    public class DashboardSummaryDto
    {
        public int TotalSupplier { get; set; }
        public int TotalAspek { get; set; }
        public int TotalParameter { get; set; }
        public int TotalKeputusan { get; set; }
        public KeputusanListDto? KeputusanTerakhir { get; set; }
        public List<SupplierDto> TopSuppliers { get; set; } = new();
    }
}
