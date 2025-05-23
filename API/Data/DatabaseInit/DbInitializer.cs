using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.DatabaseInit
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(Context db)
        {
            if (db.Database.GetPendingMigrations().Count() > 0)
            {
                await db.Database.MigrateAsync();
            }

            if (!db.Genres.Any())
            {
                // Genres
                var country = new Genre { Name = "country" };
                var pop = new Genre { Name = "pop" };
                var rock = new Genre { Name = "rock" };

                db.Genres.Add(country);
                db.Genres.Add(pop);
                db.Genres.Add(rock);

                // Artists
                var timMcGraw = new Artist
                {
                    Name = "Tim McGraw",
                    Genre = country,
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5f/Tim_McGraw_October_24_2015.jpg/330px-Tim_McGraw_October_24_2015.jpg"
                };
                db.Artists.Add(timMcGraw);

                var adele = new Artist
                {
                    Name = "Adele",
                    Genre = pop,
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/52/Adele_for_Vogue_in_2021.png/330px-Adele_for_Vogue_in_2021.png"
                };
                db.Artists.Add(adele);

                var bryanAdams = new Artist
                {
                    Name = "Bryan Adams",
                    Genre = rock,
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/20130710-ByanAdamsLucca-0019_%289273168286%29.jpg/330px-20130710-ByanAdamsLucca-0019_%289273168286%29.jpg"
                };
                db.Artists.Add(bryanAdams);

                // Albums
                var allIWant = new Album
                {
                    Name = "All I Want",
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/2/24/Timalliwant.jpg"
                };
                db.Albums.Add(allIWant);

                var album25 = new Album
                {
                    Name = "Album 25",
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/9/96/Adele_-_25_%28Official_Album_Cover%29.png"
                };
                db.Albums.Add(album25);

                var reckless = new Album
                {
                    Name = "Reckless",
                    PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/5/55/Bryan_Adams_-_Reckless_%28album_cover%29.png"
                };
                db.Albums.Add(reckless);

                // ArtistAlbumBridge
                var bryanAdamsAlbumBridge = new ArtistAlbumBridge
                {
                    Album = reckless,
                    Artist = bryanAdams
                };
                db.ArtistAlbumBridge.Add(bryanAdamsAlbumBridge);

                var adeleAlbumBridge = new ArtistAlbumBridge
                {
                    Album = album25,
                    Artist = adele
                };
                db.ArtistAlbumBridge.Add(adeleAlbumBridge);

                var timMcGrawAlbumBridge = new ArtistAlbumBridge
                {
                    Album = allIWant,
                    Artist = timMcGraw
                };
                db.ArtistAlbumBridge.Add(timMcGrawAlbumBridge);


                // Tracks
                // reference: https://pixabay.com/music/

                var sound1 = await System.IO.File.ReadAllBytesAsync("../API/Data/DatabaseInit/SoundTracks/sound1.mp3");
                var track1 = new Track
                {
                    Name = "All I want is a life",
                    Contents = sound1,
                    ContentType = "audio/mpeg",
                    Album = allIWant
                };
                db.Tracks.Add(track1);

                var sound2 = await System.IO.File.ReadAllBytesAsync("../API/Data/DatabaseInit/SoundTracks/sound2.mp3");
                var track2 = new Track
                {
                    Name = "Hello",
                    Contents = sound2,
                    ContentType = "audio/mpeg",
                    Album = album25
                };
                db.Tracks.Add(track2);

                var sound3 = await System.IO.File.ReadAllBytesAsync("../API/Data/DatabaseInit/SoundTracks/sound3.mp3");
                var track3 = new Track
                {
                    Name = "Run to you",
                    Contents = sound3,
                    ContentType = "audio/mpeg",
                    Album = reckless
                };
                db.Tracks.Add(track3);

                if (db.ChangeTracker.HasChanges())
                {
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
