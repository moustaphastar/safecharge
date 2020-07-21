using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Models;
using UI.SafeChargeModel;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private const string SECRET = "ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP";
        private const string MERCHANT_ID = "6137988932968921795";
        private const string MERCHANT_SITE_ID = "203978";
        private const string CURRENCY = "USD";
        private const string API_ROOT_ENDPOINT = "https://ppp-test.safecharge.com/ppp/api/v1/";
        private const string API_OPEN_ORDER = "https://ppp-test.safecharge.com/ppp/api/v1/openOrder.do";
        private const string API_GET_PAYMENT_STATUS = "https://ppp-test.safecharge.com/ppp/api/v1/getPaymentStatus.do";

        public ActionResult Index()
        {
            List<Product> products = GetProducts();
            return View(products);
        }

        public ActionResult Cart()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Checkout(string amount)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            if (parseAmount != true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenOrderModel openOrderModel = new OpenOrderModel()
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                Currency = CURRENCY,
                Amount = amount,
                TimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
            };

            string composedString = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, openOrderModel.Amount, CURRENCY, openOrderModel.TimeStamp, SECRET);
            openOrderModel.Checksum = GetChecksumSha256(composedString);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            };
            options.Converters.Add(new LongToStringJsonConverter());

            string inputJson = JsonSerializer.Serialize(openOrderModel, options);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.UploadString(API_OPEN_ORDER, inputJson);

            OpenOrderResultModel openOrderResult = JsonSerializer.Deserialize<OpenOrderResultModel>(json, options);
            if (openOrderResult.Status == "PENDING")
            {
                return RedirectToAction("CheckoutPending", openOrderResult);
            }
            else if (openOrderResult.Status == "ERROR")
            {
                return RedirectToAction("CheckoutError", openOrderResult);
            }

            return View(openOrderResult);
        }

        [HttpGet]
        public JsonResult GetPaymentStatus(string sessionToken)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new LongToStringJsonConverter());

            GetPaymentStatusModel getPaymentStatus = new GetPaymentStatusModel() { SessionToken = sessionToken };
            GetPaymentStatusResultModel paymentResult;
            string paymentStatus;

            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            try
            {
                string input = JsonSerializer.Serialize(getPaymentStatus);
                paymentStatus = client.UploadString(API_GET_PAYMENT_STATUS, input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            paymentResult = JsonSerializer.Deserialize<GetPaymentStatusResultModel>(paymentStatus, options);

            string status = paymentResult.Status;
            if (status == "SUCCESS")
            {
                // Handle redirect.
                //return RedirectToAction("CheckoutDone", openOrderResult); 
            }
            else if (status == "ERROR")
            {
                // Handle redirect.
                //return RedirectToAction("CheckoutError", openOrderResult); 
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Dmn()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult Dmn(string advanceResponseChecksum)
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private List<Product> GetProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Blouse", Price = 40, ImageUrl1 = "~/Content/images/p1.jpg", ImageUrl2 = "~/Content/images/p2.jpg" },
                new Product { Id = 2, Name = "Shoe", Price = 40, ImageUrl1 = "~/Content/images/p3.jpg", ImageUrl2 = "~/Content/images/p4.jpg" },
                new Product { Id = 3, Name = "Back bag", Price = 20, ImageUrl1 = "~/Content/images/p5.jpg", ImageUrl2 = "~/Content/images/p6.jpg" },
                new Product { Id = 4, Name = "Pant", Price = 30, ImageUrl1 = "~/Content/images/p7.jpg", ImageUrl2 = "~/Content/images/p8.jpg" }
                //new Product { Id = 5, Name = "Hand bag", Price = 20, ImageUrl1 = "~/Content/images/p9.jpg", ImageUrl2 = "~/Content/images/p10.jpg" },
                //new Product { Id = 6, Name = "Cap", Price = 20, ImageUrl1 = "~/Content/images/p11.jpg", ImageUrl2 = "~/Content/images/p12.jpg" },
                //new Product { Id = 7, Name = "Dress", Price = 30, ImageUrl1 = "~/Content/images/p13.jpg", ImageUrl2 = "~/Content/images/p14.jpg" },
                //new Product { Id = 8, Name = "Sunglass", Price = 50, ImageUrl1 = "~/Content/images/p15.jpg", ImageUrl2 = "~/Content/images/p16.jpg" }
            };

            return products;
        }

        public static string GetChecksumSha256(string text)
        {
            var tool = new CheckSummer();
            var result = tool.GetChecksumSha256(text);
            return result;

        }
    }
}