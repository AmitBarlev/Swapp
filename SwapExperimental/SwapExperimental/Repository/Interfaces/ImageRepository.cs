using Swap.Object.GeneralObjects;
using Swap.WebApi.Entities;

namespace Swap.WebApi.Repositories.Interfaces
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        private SwapContext _imageContext => _context as SwapContext;

        public ImageRepository(SwapContext context) : base(context) { }
    }
}
