using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingPlatform.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistID { get; set; }

        public string? PlaylistName { get; set; }

        public DateTime? CreationDate { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public User? User { get; set; }
    }
}
