using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("Parameters")]
    public class Parameter
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Aspek")]
        public Guid AspekId { get; set; }

        [Required]
        [MaxLength(50)]
        public string KodeParameter { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NamaParameter { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Satuan { get; set; } = string.Empty; // Rp, %, Hari, Skor (1-5)

        [Required]
        [MaxLength(20)]
        public string SifatKriteria { get; set; } = "Cost"; // Cost / Benefit

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bobot { get; set; } = 0;

        [MaxLength(500)]
        public string? Keterangan { get; set; }

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public Aspek? Aspek { get; set; }
        public ICollection<KeputusanNilai> KeputusanNilais { get; set; } = new List<KeputusanNilai>();
    }
}
