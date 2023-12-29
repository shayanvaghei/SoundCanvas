using System.Collections.Generic;

namespace API.DTOs
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Contents { get; set; }
        public string AlbumName { get; set; }
        public List<string> ArtistNames { get; set; }
    }
}
