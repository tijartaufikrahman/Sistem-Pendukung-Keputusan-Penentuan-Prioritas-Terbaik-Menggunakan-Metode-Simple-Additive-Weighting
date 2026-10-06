 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string KodeSupplier { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NamaSupplier { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? KontakPerson { get; set; }

        [MaxLength(50)]
        public string? Telepon { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Alamat { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Aktif"; // Aktif, Percobaan, Tidak Aktif

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation
        public ICollection<KeputusanDetail> KeputusanDetails { get; set; } = new List<KeputusanDetail>();
    }
}
