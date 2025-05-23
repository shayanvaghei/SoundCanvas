using API.Data;
using API.Repo.IRepo;
using AutoMapper;
using System.Threading.Tasks;

namespace API.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public UnitOfWork(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IAlbumRepo AlbumRepo => new AlbumRepo(_context);
        public IArtistAlbumBridgeRepo ArtistAlbumBridgeRepo => new ArtistAlbumBridgeRepo(_context);
        public IArtistRepo ArtistRepo => new ArtistRepo(_context, _mapper);
        public IGenreRepo GenreRepo => new GenreRepo(_context);
        public ITrackRepo TrackRepo => new TrackRepo(_context);

        public async Task<bool> CompleteAsync()
        {
            bool result = false;
            if (_context.ChangeTracker.HasChanges())
            {
                result = await _context.SaveChangesAsync() > 0;
            }

            return result;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
