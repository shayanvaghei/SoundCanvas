using System.Threading.Tasks;

namespace API.Repo.IRepo
{
    public interface IUnitOfWork
    {
        IAlbumRepo AlbumRepo { get; }
        IArtistRepo ArtistRepo { get; }
        IArtistAlbumBridgeRepo ArtistAlbumBridgeRepo { get; }
        IGenreRepo GenreRepo { get; }
        ITrackRepo TrackRepo { get; }

        Task<bool> CompleteAsync();
        bool HasChanges();
        void Dispose();
    }
}
