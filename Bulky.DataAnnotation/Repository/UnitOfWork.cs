using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository category { get; private set; }

        public IProductRepository product { get; private set; }
        public IShopingCartRepository shopingCart { get; private set; }

        internal readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            category = new CategoryRepository(_context);
            product = new ProductRepository(_context);
            shopingCart = new ShopingCartRepository(_context);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
