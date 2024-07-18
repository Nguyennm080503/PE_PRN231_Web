using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS
{
    public class WatercolorPaintingCreateDto
    {
        [Required]
        public string PaintingId { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[A-Z][a-zA-Z0-9 ]*$", ErrorMessage = "Painting Name must start with a capital letter and contain only letters, digits, and spaces")]
        public string PaintingName { get; set; } = null!;
        [Required]
        public string? PaintingDescription { get; set; }
        [Required]
        public string? PaintingAuthor { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public int? PublishYear { get; set; }
        [Required]
        public string? StyleId { get; set; }
    }
}
