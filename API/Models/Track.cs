using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Track
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ContentType { get; set; }
        [Required]
        public byte[] Contents { get; set; }

        [ForeignKey("Album")]
        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
