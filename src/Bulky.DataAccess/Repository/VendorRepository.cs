using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.Data;

namespace BulkyBook.DataAccess.Repository
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        private ApplicationDbContext _db;

        public VendorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Vendor vendor)
        {
            _db.Vendors.Update(vendor);
        }
    }
}
