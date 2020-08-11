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
        private const string SECRET = "8NcFYQjN5HlURMSdVef9rInFMWJRsfzr1ljw2Z8Jx7zv3AvcUwYPmXLWa9td5mt8";
        private const string MERCHANT_ID = "8813977768255734154";
        private const string MERCHANT_SITE_ID = "205848";
        private const string CLIENT_UNIQUE_ID = "203978";
        private const string User_Token_Id = "c433b5a9-bf3f";
        private readonly List<string> ALLOWED_CURRENCIES = new List<string> { "USD", "EUR" };
        private const string API_GET_SESSION_TOKEN = "https://ppp-test.safecharge.com/ppp/api/v1/getSessionToken.do";
        private const string API_OPEN_ORDER = "https://ppp-test.safecharge.com/ppp/api/v1/openOrder.do";
        private const string API_GET_PAYMENT_STATUS = "https://ppp-test.safecharge.com/ppp/api/v1/getPaymentStatus.do";
        private const string API_PAYMENT = "https://secure.safecharge.com/ppp/api/v1/payment.do";
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
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            OpenOrderResponse sessionTokenInfo = GetSessionToken(timeStamp);

            if (sessionTokenInfo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            ViewBag.TimeStmap = timeStamp;
            ViewBag.Checksum = GetChecksumString(timeStamp);
            ViewBag.ChecksumForPayment = GetChecksumString(currency, amount, timeStamp);

            return View(sessionTokenInfo);
        }

        [HttpPost]
        public ActionResult MethodUrl(string creq, string cres)
        {

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public JsonResult GetPaymentChecksum(string currency, string amount, string timeStamp)
        {
            currency = currency.ToUpper();

            string checksum = GetChecksumString(amount, currency, timeStamp);

            return Json(checksum, JsonRequestBehavior.AllowGet);
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
        #endregion

        #region Helpers
        private OpenOrderResponse GetSessionToken(string timestamp)
        {

            var model = new OpenOrderRequest
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                TimeStamp = timestamp
            };

            var str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, timestamp, SECRET);
            var checksum = GetChecksumSha256(str);

            model.Checksum = checksum;

            var requestJson = JsonSerializer.Serialize(model, JsonSerializerOptions);
            var resultJson = GetResponseFromHost(requestJson, API_GET_SESSION_TOKEN);
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
                new Product { Id = 1, Name = "Blouse", Price = "40", ImageUrl1 = "~/Content/images/p1.jpg", ImageUrl2 = "~/Content/images/p2.jpg" },
                new Product { Id = 2, Name = "Shoe", Price = "40", ImageUrl1 = "~/Content/images/p3.jpg", ImageUrl2 = "~/Content/images/p4.jpg" },
                new Product { Id = 3, Name = "Back bag", Price = "20", ImageUrl1 = "~/Content/images/p5.jpg", ImageUrl2 = "~/Content/images/p6.jpg" },
                new Product { Id = 4, Name = "Pant", Price = "30", ImageUrl1 = "~/Content/images/p7.jpg", ImageUrl2 = "~/Content/images/p8.jpg" }
            };

            return products;
        }

        private string ComposeCheckoutItemList(List<CartItem> cartItems)
        {
            int count = cartItems.Count;
            string composedString = "";

            if (count == 0)
            {
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                var item = cartItems[i];
                var n = i + 1;
                string item_name_n = string.Concat("item_name_", n, "=", item.Name);
                string item_amount_n = string.Concat("item_amount_", n, "=", item.Price);
                //string item_number_n = string.Concat("item_number_", n, "=", n);
                string item_quantity_n = string.Concat("item_quantity_", n, "=", item.Quantity);
                composedString += string.Concat(item_name_n, "&" + item_amount_n + "&" + item_quantity_n);
                //composedString += string.Concat(item_name_n, "&" + item_number_n + "&" + item_quantity_n + "&" + item_amount_n);
                if (n < count) { composedString += "&"; }
            }

            return composedString;
        }

        public string GetChecksumString(string timestamp)
        {
            var str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, timestamp, SECRET);
            var checksum = GetChecksumSha256(str);


            return checksum;
        }

        public string GetChecksumString(string currency, string amount, string timestamp)
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
            options.Converters.Add(new DecimalToStringJsonConverter());
            options.Converters.Add(new BooleanToStringJsonConverter());
            return options;
        }
        #endregion
    }
}