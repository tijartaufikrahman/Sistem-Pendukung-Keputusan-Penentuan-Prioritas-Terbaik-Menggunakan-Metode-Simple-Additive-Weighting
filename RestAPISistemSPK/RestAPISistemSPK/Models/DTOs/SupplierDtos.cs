namespace RestAPISistemSPK.Models.DTOs
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string KodeSupplier { get; set; } = string.Empty;
        public string NamaSupplier { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Alamat { get; set; }
        public string Status { get; set; } = "Aktif";
        public DateTime CreatedAt { get; set; }
        public int TotalEvaluasiDiikuti { get; set; }
    }

    public class CreateSupplierDto
    {
        public string? KodeSupplier { get; set; }
        public string NamaSupplier { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Alamat { get; set; }
        public string Status { get; set; } = "Aktif";
    }

    public class UpdateSupplierDto
    {
        public Guid Id { get; set; }
        public string? KodeSupplier { get; set; }
        public string NamaSupplier { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Alamat { get; set; }
        public string Status { get; set; } = "Aktif";
    }
}
