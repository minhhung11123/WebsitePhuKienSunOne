using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;
        private readonly dbSunOneContext _context;
        private INotyfService _notifyService { get; }

        public CheckoutController(dbSunOneContext context, INotyfService notifyService, IConfiguration config)
        {
            _context = context;
            _notifyService = notifyService;
            _clientId = config["PayPal:ClientId"];
            _clientSecret = config["PayPal:ClientSecret"];
            _mode = config["PayPal:Environment"];
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }

        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var customerID = HttpContext.Session.GetString("CustomerId");
            CheckoutVM model = new CheckoutVM();
            if (customerID != null)
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                model.CustomerId = cs.CustomerId;
                model.FullName = cs.FullName;
                model.Phone = cs.Phone;
                model.Address = cs.Address;
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
            ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "checkout")]
        public IActionResult Index(CheckoutVM checkout)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var customerID = HttpContext.Session.GetString("CustomerId");
            if (ModelState.IsValid)
            {
                if (customerID != null)
                {
                    var cs = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                    cs.FullName = checkout.FullName;
                    cs.Phone = checkout.Phone;
                    cs.Address = checkout.Address;

                    cs.LocationId = checkout.City;
                    cs.District = checkout.District;
                    cs.Ward = checkout.Ward;
                    cs.Address = checkout.Address;

                    _context.Update(cs);
                    _context.SaveChanges();
                }
                try
                {

                    //Create new order
                    Models.Order order = new Models.Order();
                    order.CustomerId = Convert.ToInt32(customerID);
                    order.City = checkout.City;
                    order.District = checkout.District;
                    order.Ward = checkout.Ward;
                    order.OrderDate = DateTime.Now;
                    order.TransactStatusId = 1;
                    order.Deleted = false;
                    order.Paid = false;
                    order.Note = checkout.Note;
                    order.TotalMoney = cart.Sum(x => x.totalMoney);
                    _context.Add(order);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("orderId", order.OrderId);

                    //Create list order detail
                    foreach (var item in cart)
                    {
                        Product pd = _context.Products.AsNoTracking().SingleOrDefault(p => p.ProductId == item.product.ProductId);
                        pd.UnitslnStock = pd.UnitslnStock - item.amount;
                        _context.Update(pd);
                        OrderDetail od = new OrderDetail();
                        od.OrderId = order.OrderId;
                        od.ProductId = item.product.ProductId;
                        od.Quantity = item.amount;
                        od.Total = item.totalMoney;
                        od.Price = item.product.Price;
                        od.Discount = item.product.Discount > 0 ? item.product.Discount : null;
                        od.CreateDate = DateTime.Now;
                        _context.Add(od);
                    }
                    _context.SaveChanges();
                    if (checkout.PaymentMethod == 2)
                    {
                        return RedirectToAction("PaymentWithPaypal", "Checkout");
                    }
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("orderId");
                    return RedirectToAction("Confirmation");
                }
                catch
                {
                    ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
                    ViewBag.Cart = cart;
                    return View(checkout);
                }
            }
            ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
            ViewBag.Cart = cart;
            return View(checkout);
        }

        [Route("confirmation.html", Name = "Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }

        [Route("PaymentFailed.html", Name = "PaymentFailed")]
        public IActionResult PaymentFailed()
        {
            return View();
        }

        [Route("PaymentWithPaypal", Name = "PaymentWithPaypal")]
        public async Task<ActionResult> PaymentWithPaypalAsync(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext(_clientId, _clientSecret, _mode);
            try
            {
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURL = this.Request.Scheme + "://" + this.Request.Host + "/PaymentWithPaypal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    var createdPayment = this.CreatePayment(apiContext, baseURL + "guid=" + guid, "");
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var paymentId = HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaymentFailed");
                    }
                    var orderId = HttpContext.Session.GetInt32("orderId");
                    var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
                    order.PaymentDate = DateTime.Now;
                    order.PaymentId = paymentId;
                    order.Paid = true;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("orderId");
                    return View("Confirmation");
                }
            }
            catch
            {
                return View("PaymentFailed");
            }
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId,
            };
            this.payment = new Payment()
            {
                id = paymentId,
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            foreach (var item in cart)
            {
                itemList.items.Add(new Item()
                {
                    name = item.product.ProductName,
                    currency = "USD",
                    price = Math.Round((decimal)((item.product.Discount > 0 ? item.product.Discount : item.product.Price) / ExchangeRate.GetUSDBuyRate()), 2).ToString(),
                    quantity = item.amount.ToString(),
                    sku = "SKU"
                });
            }
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            var amount = new Amount()
            {
                currency = "USD",
                total = (cart.Sum(x => x.totalMoney) / ExchangeRate.GetUSDBuyRate()).ToString("0.00"),
            };
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }
    }
}
