namespace RestAPISistemSPK.Models.DTOs
{
    // ASPEK DTOs
    public class AspekDto
    {
        public Guid Id { get; set; }
        public string KodeAspek { get; set; } = string.Empty;
        public string NamaAspek { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public int Urutan { get; set; }
        public int JumlahParameter { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ParameterDto> Parameters { get; set; } = new();
    }

    public class CreateAspekDto
    {
        public string? KodeAspek { get; set; }
        public string NamaAspek { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public int Urutan { get; set; } = 1;
    }

    public class UpdateAspekDto
    {
        public Guid Id { get; set; }
        public string? KodeAspek { get; set; }
        public string NamaAspek { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public int Urutan { get; set; } = 1;
    }

    // PARAMETER DTOs
    public class ParameterDto
    {
        public Guid Id { get; set; }
        public Guid AspekId { get; set; }
        public string? NamaAspek { get; set; }
        public string KodeParameter { get; set; } = string.Empty;
        public string NamaParameter { get; set; } = string.Empty;
        public string Satuan { get; set; } = string.Empty;
        public string SifatKriteria { get; set; } = "Cost"; // Cost / Benefit
        public decimal Bobot { get; set; } = 0;
        public string? Keterangan { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateParameterDto
    {
        public Guid AspekId { get; set; }
        public string? KodeParameter { get; set; }
        public string NamaParameter { get; set; } = string.Empty;
        public string Satuan { get; set; } = string.Empty;
        public string SifatKriteria { get; set; } = "Cost";
        public decimal Bobot { get; set; } = 0;
        public string? Keterangan { get; set; }
    }

    public class UpdateParameterDto
    {
        public Guid Id { get; set; }
        public Guid AspekId { get; set; }
        public string? KodeParameter { get; set; }
        public string NamaParameter { get; set; } = string.Empty;
        public string Satuan { get; set; } = string.Empty;
        public string SifatKriteria { get; set; } = "Cost";
        public decimal Bobot { get; set; } = 0;
        public string? Keterangan { get; set; }
    }

    public class ParameterBobotItemDto
    {
        public Guid Id { get; set; }
        public decimal Bobot { get; set; }
    }
}
