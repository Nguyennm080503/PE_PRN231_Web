using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS
{
    public class WatercolorPaintingViewDto
    {
        public string PaintingId { get; set; } = null!;

        public string PaintingName { get; set; } = null!;

        public string? PaintingDescription { get; set; }

        public string? PaintingAuthor { get; set; }

        public decimal? Price { get; set; }

        public int? PublishYear { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? StyleID { get; set; }

        public virtual string? StyleName { get; set; }
    }
}
