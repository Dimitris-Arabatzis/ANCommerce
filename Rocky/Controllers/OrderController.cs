using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(
            IOrderHeaderRepository orderHRepo,
            IOrderDetailRepository orderDRepo,
            IBrainTreeGate brain)
        {
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }
        public IActionResult Index(string searchName=null,string searchEmail=null,string searchPhone=null,string Status=null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepo.GetAll(),
                StatusList = WebConstants.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(x => x.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(x => x.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status!= "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(x => x.OrderStatus.ToLower().Contains(Status.ToLower()));
            }
            return View(orderListVM);
        }


        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(x => x.Id == id),
                OrderDetail = _orderDRepo.GetAll(x => x.OrderHeaderId == id, includeProperies: "Product")
            };

            return View(OrderVM);

        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstants.StatusProcessing;
            _orderHRepo.Save();
            TempData[WebConstants.Success] = "Order In Process!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstants.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepo.Save();
            TempData[WebConstants.Success] = "Order Shipped Successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);

            var gateway = _brain.GetGateway();

            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);
            if(transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund 
                Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }
            orderHeader.OrderStatus = WebConstants.StatusRefunded;
            TempData[WebConstants.Success] = "Order Refunded Successfully!";

            _orderHRepo.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHRepo.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;
            _orderHRepo.Save();
            TempData[WebConstants.Success] = "Order Details Updated Successfully!";
            return RedirectToAction("Details","Order",new { id=orderHeaderFromDb.Id});
        }
        
    }
}
