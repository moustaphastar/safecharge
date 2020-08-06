using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web.Mvc;
using UI.Helpers;
using UI.Models;
using UI.SafeChargeModels;
//using Newtonsoft.Json;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private const string SECRET = "8NcFYQjN5HlURMSdVef9rInFMWJRsfzr1ljw2Z8Jx7zv3AvcUwYPmXLWa9td5mt8"; //"XlfztkmqgLcGeW3brWNlpwne3u6YyXPpb3IUpLyo2pK7q1SzHWiRoRNCXJRoFaNv"; //"ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP";
        private const string MERCHANT_ID = "8813977768255734154"; //"5305553900704185318"; //"6137988932968921795";
        private const string MERCHANT_SITE_ID = "205848"; //"205838"; //"203978";
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
        public ActionResult Checkout(string amount, string currency, string cartItems)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            currency = currency.ToUpper();
            //if (parseAmount != true || ALLOWED_CURRENCIES.Contains(currency.ToUpper()))//.Contains(currency.ToUpper()))
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // "2020-08-05 01:55:33"

            // Prepare view model.
            CheckoutViewModel model = new CheckoutViewModel()
            {
                Amount = amount,
                Checksum = "",
                Currency = currency,
                ItemListString = "",
                Items = JsonSerializer.Deserialize<List<CartItem>>(cartItems, JsonSerializerOptions),
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                TimeStamp = timeStamp,
                UserTokenId = User_Token_Id,
                Version = "4.0.0", // TODO: Bug: Host returns 1.0.0 which fails processing.
            };

            // Produce view model's checksum.
            model.Items.ForEach(x => model.ItemListString += string.Concat(x.Name, x.Price, x.Quantity));
            string str = string.Concat(SECRET,
                MERCHANT_ID,
                MERCHANT_SITE_ID,
                model.Version,
                User_Token_Id,
                model.Items.Count,
                "0000", // For simplicity no extra fee is used
                currency,
                model.ItemListString,
                amount,
                "cc_card", // payment method
                "en_US", // merchant locale
                "sfchargeapp9@gmail.com", //email
                "CA", // country code
                "https://sandbox.safecharge.com/lib/demo_process_request/response.php", //notify
                "https://sandbox.safecharge.com/lib/demo_process_request/response.php", //success
                "https://ppp-test.safecharge.com/ppp/defaultCancel.do", //error
                "https://sandbox.safecharge.com/lib/demo_process_request/response.php", //notify
                timeStamp);
            model.Checksum = GetChecksumSha256(str);
            return View(model);
        }

        [HttpGet]
        public ActionResult CheckoutWithSDK(string amount, string currency, bool threeD = false)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            bool currencyAllowed = ALLOWED_CURRENCIES.Contains(currency.ToUpper());
            //if (parseAmount != true || currencyAllowed != true)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            // Prepare model for openOrder() api request. // TODO: Bug? : ClientUniqueId null is accepted.
            OpenOrderRequest openOrderModel = new OpenOrderRequest()
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                Currency = currency.ToUpper(),
                Amount = amount,
                TimeStamp = timeStamp,
                Checksum = GetChecksumSha256(string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, amount, currency.ToUpper(), timeStamp, SECRET))
            };

            // Send a request to openOrder() api call to open a session.
            string openOrderModelJson = JsonSerializer.Serialize(openOrderModel, JsonSerializerOptions);
            string openOrderResponse = GetResponseFromHost(openOrderModelJson, API_OPEN_ORDER);

            // If response null, return service unavailable result.
            if (openOrderResponse == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
            }

            // Deserialize openOrder() api response.
            OpenOrderResponse openOrderResult = JsonSerializer.Deserialize<OpenOrderResponse>(openOrderResponse, JsonSerializerOptions);

            // Raise error if openOrder() api response status value is ERROR.
            if (openOrderResult.Status == "ERROR")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            if (threeD == true)
            {
                return View("Checkout3d", openOrderResult);
            }
            return View(openOrderResult);
        }

        public JsonResult Payment(string authenticate3dResult, string amount, string currency, string sessionToken,
            string requestId, string uniqeId, string orderId, string cardholderName, string CVV, string cavv,
            string exi, string cardnumber, string expmonth, string expyear, string data)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            //bool currencyAllowed = ALLOWED_CURRENCIES.Contains(currency.ToUpper());
            //if (parseAmount != true || currencyAllowed != true)
            //{
            //    return Json("Error", JsonRequestBehavior.AllowGet);
            //}

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            // Prepare model for openOrder() api request. // TODO: Bug? : ClientUniqueId null is accepted.
            SessionTokenRequest sessionInfoToken = new SessionTokenRequest()
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                ClientRequestId = requestId,
                TimeStamp = timeStamp,
                Checksum = GetChecksumSha256(string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, requestId, timeStamp, SECRET))
            };

            // Send a request to openOrder() api call to open a session.
            //string sessionInfoJson = JsonSerializer.Serialize(sessionInfoToken, JsonSerializerOptions);
            //string openOrderResponse = GetResponseFromHost(sessionInfoJson, API_GET_SESSION_TOKEN);

            //OpenOrderResponse sessionInfo = JsonSerializer.Deserialize<OpenOrderResponse>(openOrderResponse, JsonSerializerOptions);
            //Authenticate3dResult authenticate3dInfo = JsonSerializer.Deserialize<Authenticate3dResult>(authenticate3dResult, JsonSerializerOptions);

            //PaymentRequest request = new PaymentRequest
            //{
            //    Amount = amount,
            //    ClientRequestId = sessionInfo.ClientRequestId,
            //    ClientUniqueId = sessionInfo.ClientUniqueId,
            //    Currency = currency,
            //    MerchantId = MERCHANT_ID,
            //    MerchantSiteId = MERCHANT_SITE_ID,
            //    OrderId = sessionInfo.OrderId,
            //    SessionToken = sessionInfo.SessionToken,
            //    TimeStamp = timeStamp,
            //    UserTokenId = sessionInfo.UserTokenId,
            //};

            //request.DeviceDetails = new DeviceDetails
            //{
            //    IpAddress = "93.146.254.172"
            //};

            //request.PaymentOptions = new PaymentOptions
            //{
            //    Card = new Cards
            //    {
            //        CardHolderName = cardholderName,
            //        CVV = CVV,
            //        CardNumber = "4000027891380961", //cardnumber,
            //        ExpirationMonth = "12",//expmonth,
            //        ExpirationYear = "22" //expyear,
            //    }
            //};

            //request.BillingAddress = new BillingAddress
            //{
            //    Country = "GB",
            //    Email = "john.smith@safecharge.com"
            //};

            //string str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, amount, currency, request.TimeStamp, SECRET);
            //request.Checksum = GetChecksumSha256(str);

            //string paymentModelRequest = JsonSerializer.Serialize(request, JsonSerializerOptions);
            //string paymentResponse = GetResponseFromHost(paymentModelRequest, API_PAYMENT);

            string req = GetResponseFromHost(data, API_PAYMENT);

            //if (paymentResponse == null)
            //{
            //    return Json("Error", JsonRequestBehavior.AllowGet);
            //}

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPaymentStatus(string sessionToken)
        {
            try
            {
                string paymentStatusJson = JsonSerializer.Serialize(new PaymentStatusRequest() { SessionToken = sessionToken });
                string paymentStatusResponse = GetResponseFromHost(API_GET_PAYMENT_STATUS, paymentStatusJson);

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

        [HttpGet]
        public JsonResult GetChecksum(string amount, string currency)
        {
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var str = string.Concat(MERCHANT_ID, MERCHANT_SITE_ID, amount, currency, timeStamp, SECRET);
            var checksum = GetChecksumSha256(str);


            return Json(new { checksum, timeStamp }, JsonRequestBehavior.AllowGet);
        }

        #region Helpers
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