using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
