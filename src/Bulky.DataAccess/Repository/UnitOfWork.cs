using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBookWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IVendorRepository Vendor { get; private set; }
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            Product = new ProductRepository(db);
            Vendor = new VendorRepository(db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
