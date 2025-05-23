using API.Data;
using API.Models;
using API.Repo.IRepo;

namespace API.Repo
{
    public class GenreRepo : BaseRepo<Genre>, IGenreRepo
    {
        public GenreRepo(Context context) : base(context)
        {
            
        }
    }
}
