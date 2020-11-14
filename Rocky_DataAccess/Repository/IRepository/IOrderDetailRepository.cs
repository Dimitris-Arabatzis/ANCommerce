using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
