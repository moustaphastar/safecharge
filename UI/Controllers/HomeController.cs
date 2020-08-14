using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web.Mvc;
using UI.Helpers;
using UI.Models;
using UI.SafeChargeModels;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private const string SECRET = "ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP";
        private const string MERCHANT_ID = "6137988932968921795";
        private const string MERCHANT_SITE_ID = "203978";
        private readonly List<string> ALLOWED_CURRENCIES = new List<string> { "USD", "EUR" };
        private const string API_OPEN_ORDER = "https://ppp-test.safecharge.com/ppp/api/v1/openOrder.do";
        private const string API_GET_PAYMENT_STATUS = "https://ppp-test.safecharge.com/ppp/api/v1/getPaymentStatus.do";
        private const string API_PURCHASE = "https://ppp-test.safecharge.com/ppp/purchase.do?";
        private readonly JsonSerializerOptions JsonSerializerOptions = GetJsonSerializerOptions();
        #endregion

        #region ActionResults
        [HttpGet]
        public ActionResult Index()
        {
            List<Product> products = GetProducts();
            return View(products);
        }

        [HttpGet]
        public ActionResult Cart()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Checkout(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            bool currencyAllowed = ALLOWED_CURRENCIES.Contains(currency.ToUpper());
            if (parseAmount != true || currencyAllowed != true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            OpenOrderResponse openOrderResponse = OpenOrder(amount, currency, timeStamp);

            // If response null, return service unavailable result.
            if (openOrderResponse == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
            }

            ViewBag.Amount = amount;
            ViewBag.Currency = currency.ToUpper();
            return View(openOrderResponse);
        }

        [HttpPost]
        public JsonResult GetPaymentStatus(string sessionToken)
        {
            var paymentStatusRequest = new PaymentStatusRequest
            {
                SessionToken = sessionToken
            };

            try
            {
                string paymentStatusJson = JsonSerializer.Serialize(paymentStatusRequest, JsonSerializerOptions);
                string paymentStatusResponse = GetResponseFromHost(paymentStatusJson, API_GET_PAYMENT_STATUS);

                // If response null, return service unavailable result.
                if (paymentStatusResponse == null)
                {
                    throw new Exception("Service Unavailable");
                }

                var paymentResult = JsonSerializer.Deserialize<PaymentStatusResponse>(paymentStatusResponse, JsonSerializerOptions);

                return Json(paymentResult.Status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Dmn()
        {
            // TODO: DMN configuration is not allowed by sandbox currently.
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult Dmn(string advanceResponseChecksum)
        {
            // TODO: DMN configuration is not allowed by sandbox currently.
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region Helpers
        private OpenOrderResponse OpenOrder(string amount, string currency, string timestamp)
        {
            OpenOrderRequest request = new OpenOrderRequest()
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                Currency = currency.ToUpper(),
                Amount = amount,
                TimeStamp = timestamp,
                Checksum = GetChecksumString(amount, currency.ToUpper(), timestamp)
            };

            var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);
            var resultJson = GetResponseFromHost(requestJson, API_OPEN_ORDER);
            var result = JsonSerializer.Deserialize<OpenOrderResponse>(resultJson, JsonSerializerOptions);

            return result;
        }

        private string GetResponseFromHost(string serializedData, string address)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                string result = client.UploadString(address, serializedData);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<Product> GetProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Blouse", Price = 40, ImageUrl1 = "~/Content/images/p1.jpg", ImageUrl2 = "~/Content/images/p2.jpg" },
                new Product { Id = 2, Name = "Shoe", Price = 40, ImageUrl1 = "~/Content/images/p3.jpg", ImageUrl2 = "~/Content/images/p4.jpg" },
                new Product { Id = 3, Name = "Back bag", Price = 20, ImageUrl1 = "~/Content/images/p5.jpg", ImageUrl2 = "~/Content/images/p6.jpg" },
                new Product { Id = 4, Name = "Pant", Price = 30, ImageUrl1 = "~/Content/images/p7.jpg", ImageUrl2 = "~/Content/images/p8.jpg" }
            };

            return products;
        }

        public string GetChecksumString(string timestamp)
        {
            var str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, timestamp, SECRET);
            var checksum = GetChecksumSha256(str);

            return checksum;
        }

        public string GetChecksumString(string amount, string currency, string timestamp)
        {
            var str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, amount, currency.ToUpper(), timestamp, SECRET);
            var checksum = GetChecksumSha256(str);

            return checksum;
        }

        private string GetChecksumSha256(string text)
        {
            var provider = new CheckSumProvider();
            var checksum = provider.GetChecksumSha256(text);
            return checksum;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new LongToStringJsonConverter());
            return options;
        }

        #endregion
    }
}