using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingPlatform.Models
{
    public class PlaylistTrack
    {
        [Key]
        public int PlaylistTrackID { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistID { get; set; }

        [ForeignKey("Track")]
        public int TrackID { get; set; }

        public DateTime AddedDate { get; set; }

        public Playlist Playlist { get; set; }

        public Track Track { get; set; }
    }
}
