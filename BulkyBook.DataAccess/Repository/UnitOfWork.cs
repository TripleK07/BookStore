using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public UnitOfWork(ApplicationDbContext context) { 
            this.dbContext = context;
            this.Category = new CategoryRepository(dbContext);
            this.CoverType = new CoverTypeRepository(dbContext);
            this.Product = new ProductRepository(dbContext);
            this.ApplicationUser = new ApplicationUserRepository(dbContext);
        }

        public void Save()
        {
            dbContext.SaveChanges();   
        }
    }
}
