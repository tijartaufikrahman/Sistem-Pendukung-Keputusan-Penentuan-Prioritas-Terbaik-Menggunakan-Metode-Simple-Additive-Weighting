using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("Keputusans")]
    public class Keputusan
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string KodeKeputusan { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NamaKeputusan { get; set; } = string.Empty;

        public DateTime TanggalKeputusan { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Keterangan { get; set; }

        [ForeignKey("Aspek")]
        public Guid? AspekId { get; set; }

        // Pemenang Terpilih
        [ForeignKey("SupplierPemenang")]
        public Guid? SupplierPemenangId { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? SkorTertinggi { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Selesai"; // Draft, Selesai

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public Aspek? Aspek { get; set; }
        public Supplier? SupplierPemenang { get; set; }
        public ICollection<KeputusanDetail> KeputusanDetails { get; set; } = new List<KeputusanDetail>();
    }
}
