using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocky_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
    }
}
