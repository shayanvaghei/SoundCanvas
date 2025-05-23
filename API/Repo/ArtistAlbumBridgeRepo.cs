using API.Data;
using API.Models;
using API.Repo.IRepo;

namespace API.Repo
{
    public class ArtistAlbumBridgeRepo : BaseRepo<ArtistAlbumBridge>, IArtistAlbumBridgeRepo
    {
        public ArtistAlbumBridgeRepo(Context context) : base(context)
        {

        }
    }
}
