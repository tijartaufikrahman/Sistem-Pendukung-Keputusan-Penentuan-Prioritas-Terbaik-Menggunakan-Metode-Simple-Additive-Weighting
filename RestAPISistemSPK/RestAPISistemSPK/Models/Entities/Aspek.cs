using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPISistemSPK.Models.Entities
{
    [Table("Aspeks")]
    public class Aspek
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string KodeAspek { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NamaAspek { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Deskripsi { get; set; }

        public int Urutan { get; set; } = 1;

        // Metadata Audit (Independen)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation (Hierarki ke Parameter)
        public ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
