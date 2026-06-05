using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingPlatform.Models
{
    public class Track
    {
        [Key]
        public int TrackID { get; set; }

        public string? Title { get; set; }

        public int? Duration { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}
