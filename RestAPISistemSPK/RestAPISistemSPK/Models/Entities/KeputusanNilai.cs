using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("KeputusanNilais")]
    public class KeputusanNilai
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("KeputusanDetail")]
        public Guid KeputusanDetailId { get; set; }

        [ForeignKey("Parameter")]
        public Guid ParameterId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiRiil { get; set; } = 0; // Fakta nilai penawaran (misal: Rp 12000, 2 Hari, 1%)

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        // Navigation
        public KeputusanDetail? KeputusanDetail { get; set; }
        public Parameter? Parameter { get; set; }
    }
}
