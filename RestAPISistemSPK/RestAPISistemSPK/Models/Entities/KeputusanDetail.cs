using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("KeputusanDetails")]
    public class KeputusanDetail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Keputusan")]
        public Guid KeputusanId { get; set; }

        [ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? SkorAkhir { get; set; }

        public int? Peringkat { get; set; }

        [MaxLength(100)]
        public string? StatusRekomendasi { get; set; } = "Draft"; // Draft, Pilihan Utama, Alternatif Cadangan, Tidak Direkomendasikan

        [MaxLength(1000)]
        public string? CatatanEvaluasi { get; set; }

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        // Navigation
        public Keputusan? Keputusan { get; set; }
        public Supplier? Supplier { get; set; }
        public ICollection<KeputusanNilai> KeputusanNilais { get; set; } = new List<KeputusanNilai>();
    }
}
