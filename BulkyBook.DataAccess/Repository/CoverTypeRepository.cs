using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext dbContext;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            dbContext = context;
        }

        public void Update(CoverType obj)
        {
            dbContext.Update(obj);
        }
    }
}
