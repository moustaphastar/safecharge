﻿using System;
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
        private const string SECRET = "XlfztkmqgLcGeW3brWNlpwne3u6YyXPpb3IUpLyo2pK7q1SzHWiRoRNCXJRoFaNv"; //"ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP";
        private const string MERCHANT_ID = "5305553900704185318"; //"6137988932968921795";
        private const string MERCHANT_SITE_ID = "205838"; //"203978";
        private const string CLIENT_UNIQUE_ID = "203978";
        private const string User_Token_Id = "c433b5a9-bf3f";
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
        public ActionResult CheckoutWithSDK(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            bool currencyAllowed = ALLOWED_CURRENCIES.Contains(currency.ToUpper());
            if (parseAmount != true || currencyAllowed != true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            // Prepare model for openOrder() api request. // TODO: Bug? : ClientUniqueId null is accepted.
            OpenOrderRequestWebSDK openOrderModel = new OpenOrderRequestWebSDK()
            {
                MerchantId = MERCHANT_ID,
                MerchantSiteId = MERCHANT_SITE_ID,
                //ClientUniqueId = CLIENT_UNIQUE_ID,
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
            return View(openOrderResult);
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
            return options;
        }

        #endregion
    }
}