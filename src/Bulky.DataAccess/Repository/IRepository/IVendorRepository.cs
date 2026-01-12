using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        void Update(Vendor vendor);
    }
}
