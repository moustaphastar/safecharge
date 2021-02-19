using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Safecharge;
using Safecharge.Model.Common;
using Safecharge.Model.PaymentOptionModels;
using Safecharge.Model.PaymentOptionModels.CardModels;
using Safecharge.Model.PaymentOptionModels.InitPayment;
using Safecharge.Model.PaymentOptionModels.ThreeDModels;
using Safecharge.Request;
using Safecharge.Response.Payment;
using Safecharge.Utils;
using Safecharge.Utils.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApp.Helper;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISafecharge _safecharge;
        private const string KEY = "0Sh4F6R2jQo6h8GYv62tabIOdgR4jqr2s41OMuMsLlB9RVFwmDeRtORN9m0Opj0W";
        private const string MERCHANTID = "2153502749988995611";
        private const string SITEID = "213146";
        private const string UNIQUEID = "12345";
        private const string REQUESTID = "20200807235612";

        public HomeController(ILogger<HomeController> logger, ISafecharge safecharge)
        {
            _logger = logger;
            _safecharge = safecharge;
        }

        public IActionResult Index()
        {
            List<Product> products = GetProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult Cart()
        {
            return View();
        }

        // Non 3D Scenario
        [HttpGet]
        public IActionResult CheckoutNon3D(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);

            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);
            var re = new SafechargeRequestExecutor();
            var sessionTokenReq = new GetSessionTokenRequest(merchantInfo);
            var sessionTokenRes = re.GetSessionToken(sessionTokenReq).GetAwaiter().GetResult();
            if (sessionTokenRes == null) return RedirectToAction("Error");
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            var sessionTokenReqJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenReq);
            Console.WriteLine(sessionTokenReqJson);
            var sessionTokenResponseJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenRes);
            Console.WriteLine(sessionTokenResponseJson);
            return View(sessionTokenRes);
        }


        // 3D Secure Authentication Only Flow
        [HttpGet]
        public IActionResult Checkout(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);
            var safecharge = new Safecharge.Safecharge(merchantInfo);
            var openOrderResponse = safecharge.OpenOrder(
                "USD",
                "200",
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });
            if (openOrderResponse.Result == null) return RedirectToAction("Error");
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            ViewBag.TimeStamp = timestamp;
            ViewBag.Checksum = GetChecksumString(currency, amount, timestamp);

            return View(openOrderResponse.Result);
        }

        // 3D Secure Server to Server Flow Controller
        [HttpGet]
        public IActionResult CheckoutS2S(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);
            var safecharge = new Safecharge.Safecharge(merchantInfo);
            var openOrderResponse = safecharge.OpenOrder(
                "USD",
                "200",
                userTokenId: "TestUserTokenId",
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }); ; ;
            if (openOrderResponse.Result == null) return RedirectToAction("Error");
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            ViewBag.TimeStamp = timestamp;
            ViewBag.Checksum = GetChecksumString(currency, amount, timestamp);

            return View(openOrderResponse.Result);






            //// Parse incoming string as decimal.
            //bool parseAmount = decimal.TryParse(amount, out decimal _amount);

            //// If parsing is not succeeded redirect to Error action.
            //if (!parseAmount) { return RedirectToAction("Error"); }

            //// Initiate SafechargeRequestExecutor.
            //var re = new SafechargeRequestExecutor();

            //// Initiate Safecharge's MerchantInfo class to pass GetSessionTokenRequest 
            //var merchantInfo = new MerchantInfo(
            //    merchantKey: KEY,
            //    merchantId: MERCHANTID,
            //    merchantSiteId: SITEID,
            //    serverHost: ApiConstants.IntegrationHost,
            //    hashAlgorithm: HashAlgorithmType.SHA256);

            //// Initiate GetSessionTokenRequest with MerchantInfo argument.
            //var sessionTokenReq = new GetSessionTokenRequest(merchantInfo);
            //var sessionTokenReqJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenReq);
            //Console.WriteLine(sessionTokenReqJson);
            //// Execute request and get the awaited result.
            //var sessionTokenResponse = re.GetSessionToken(sessionTokenReq).GetAwaiter().GetResult();
            //var sessionTokenResponseJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenResponse);
            //Console.WriteLine(sessionTokenResponseJson);
            //// If response is null redirect to Error action.
            //if (sessionTokenResponse == null) return RedirectToAction("Error");

            //// Viewbags
            //ViewBag.AuthorizedAmount = amount;
            //ViewBag.Currency = currency.ToUpper();

            //return View(sessionTokenResponse);
        }

        [HttpGet]
        public IActionResult CheckoutMPIOnly(string amount, string currency)
        {
            // Parse incoming string as decimal.
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);

            // If parsing is not succeeded redirect to Error action.
            if (!parseAmount) { return RedirectToAction("Error"); }

            // Initiate SafechargeRequestExecutor.
            var re = new SafechargeRequestExecutor();

            // Initiate Safecharge's MerchantInfo class to pass GetSessionTokenRequest 
            var merchantInfo = new MerchantInfo(
                merchantKey: KEY,
                merchantId: MERCHANTID,
                merchantSiteId: SITEID,
                serverHost: ApiConstants.IntegrationHost,
                hashAlgorithm: HashAlgorithmType.SHA256);

            // Initiate GetSessionTokenRequest with MerchantInfo argument.
            var sessionTokenReq = new GetSessionTokenRequest(merchantInfo);
            var sessionTokenReqJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenReq);
            Console.WriteLine(sessionTokenReqJson);
            // Execute request and get the awaited result.
            var sessionTokenResponse = re.GetSessionToken(sessionTokenReq).GetAwaiter().GetResult();
            var sessionTokenResponseJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenResponse);
            Console.WriteLine(sessionTokenResponseJson);
            // If response is null redirect to Error action.
            if (sessionTokenResponse == null) return RedirectToAction("Error");

            // Viewbags
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();

            return View(sessionTokenResponse);
        }

        [HttpGet]
        public IActionResult CheckoutZeroAuth(string amount, string currency)
        {
            // Parse incoming string as decimal.
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);

            // Initiate Safecharge. 
            var safecharge = new Safecharge.Safecharge(
                merchantKey: KEY,
                merchantId: MERCHANTID,
                siteId: SITEID,
                serverHost: ApiConstants.IntegrationHost,
                algorithmType: HashAlgorithmType.SHA256);

            // Open order with 0 amount and Auth transactionType.
            var openOrderResponse = safecharge.OpenOrder(
                currency: "USD",
                amount: "0",
                transactionType: "Auth",
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                userTokenId: "ZeroUser2",
                //authenticationTypeOnly: "RECURRING",
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });

            // If response is null redirect to Error action.
            if (openOrderResponse.Result == null) return RedirectToAction("Error");
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            var openOrderResponseJson = System.Text.Json.JsonSerializer.Serialize(openOrderResponse.Result);
            Console.WriteLine(openOrderResponseJson);
            return View(openOrderResponse.Result);
        }

        [HttpGet]
        public IActionResult CheckoutAuthSettle(string amount, string currency)
        {
            bool parseAmount = decimal.TryParse(amount, out decimal _amount);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);
            var safecharge = new Safecharge.Safecharge(merchantInfo);
            var openOrderResponse = safecharge.OpenOrder(
                currency: "USD",
                amount: "180",
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }).GetAwaiter().GetResult();
            if (openOrderResponse == null) return RedirectToAction("Error");
            var sessionTokenResponseJson = System.Text.Json.JsonSerializer.Serialize(openOrderResponse);
            Console.WriteLine(sessionTokenResponseJson);
            ViewBag.AuthorizedAmount = amount;
            ViewBag.Currency = currency.ToUpper();
            ViewBag.TimeStamp = timestamp;
            ViewBag.Checksum = GetChecksumString(currency, amount, timestamp);

            return View(openOrderResponse);
        }



        [HttpPost]
        public IActionResult InitPayment([FromBody] InitPaymentRequest request)
        {
            var initPaymentResponse = _safecharge.InitPayment(
                currency: request.Currency,
                amount: request.Amount,
                new InitPaymentPaymentOption
                {
                    Card = new InitPaymentCard
                    {
                        CardNumber = request.PaymentOption.Card.CardNumber,
                        CardHolderName = request.PaymentOption.Card.CardHolderName,
                        ExpirationMonth = request.PaymentOption.Card.ExpirationMonth,
                        ExpirationYear = request.PaymentOption.Card.ExpirationYear,
                        CVV = request.PaymentOption.Card.CVV,
                        ThreeD = new InitPaymentThreeD
                        {
                            MethodNotificationUrl = request.PaymentOption.Card.ThreeD.MethodNotificationUrl,
                        }
                    }
                },
                //userTokenId: "<a user identifier>",
                orderId: "33704071",
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }).GetAwaiter().GetResult();

            // NON-3.
            if (initPaymentResponse.PaymentOption.Card.ThreeD.V2supported == "false")
            {
                var paymentResponseNon3d = _safecharge.Payment(
                    request.Currency,
                    request.Amount,
                    new PaymentOption
                    {
                        Card = new Card
                        {
                            CardNumber = request.PaymentOption.Card.CardNumber,
                            CardHolderName = request.PaymentOption.Card.CardHolderName,
                            ExpirationMonth = request.PaymentOption.Card.ExpirationMonth,
                            ExpirationYear = request.PaymentOption.Card.ExpirationYear,
                            CVV = request.PaymentOption.Card.CVV,
                            ThreeD = new ThreeD
                            {
                                MethodCompletionInd = "U", // responseOne returned null for methorUrl which means fingerprinting not needed.
                                Version = "2.1.0",
                                NotificationURL = request.PaymentOption.Card.ThreeD.MethodNotificationUrl,
                                MerchantURL = "<merchantURL>",
                                PlatformType = "02",
                                V2AdditionalParams = new V2AdditionalParams
                                {
                                    ChallengeWindowSize = "05"
                                },
                                BrowserDetails = new BrowserDetails
                                {
                                    AcceptHeader = "text/html,application/xhtml+xml",
                                    Ip = "192.168.1.11",
                                    JavaEnabled = "TRUE",
                                    JavaScriptEnabled = "TRUE",
                                    Language = "EN",
                                    ColorDepth = "48",
                                    ScreenHeight = "400",
                                    ScreenWidth = "600",
                                    TimeZone = "0",
                                    UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47"
                                }
                            }
                        }
                    },
                    clientUniqueId: UNIQUEID,
                    clientRequestId: REQUESTID,
                    userTokenId: initPaymentResponse.UserTokenId,
                    relatedTransactionId: initPaymentResponse.TransactionId,
                    billingAddress: new UserAddress
                    {
                        Country = "US",
                        Email = "john.smith@email.com",
                    },
                    deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }).GetAwaiter().GetResult();
                return View("PaymentResult", paymentResponseNon3d);
            }
            else// 3D supported
            {
                // No fringerprinting...
                if (initPaymentResponse?.PaymentOption?.Card?.ThreeD?.MethodUrl.Length == 0)
                {
                    var paymentResponseOne = _safecharge.Payment(
                        request.Currency,
                        request.Amount,
                        new PaymentOption
                        {
                            Card = new Card
                            {
                                CardNumber = request.PaymentOption.Card.CardNumber,
                                CardHolderName = request.PaymentOption.Card.CardHolderName,
                                ExpirationMonth = request.PaymentOption.Card.ExpirationMonth,
                                ExpirationYear = request.PaymentOption.Card.ExpirationYear,
                                CVV = request.PaymentOption.Card.CVV,
                                ThreeD = new ThreeD
                                {
                                    MethodCompletionInd = "U", // responseOne returned null for methorUrl which means fingerprinting not needed.
                                    Version = "2.1.0",
                                    NotificationURL = request.PaymentOption.Card.ThreeD.MethodNotificationUrl,
                                    MerchantURL = "<merchantURL>",
                                    PlatformType = "02",
                                    V2AdditionalParams = new V2AdditionalParams
                                    {
                                        ChallengeWindowSize = "05"
                                    },
                                    BrowserDetails = new BrowserDetails
                                    {
                                        AcceptHeader = "text/html,application/xhtml+xml",
                                        Ip = "192.168.1.11",
                                        JavaEnabled = "TRUE",
                                        JavaScriptEnabled = "TRUE",
                                        Language = "EN",
                                        ColorDepth = "48",
                                        ScreenHeight = "400",
                                        ScreenWidth = "600",
                                        TimeZone = "0",
                                        UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47"
                                    }
                                }
                            }
                        },
                        clientUniqueId: UNIQUEID,
                        clientRequestId: REQUESTID,
                        userTokenId: initPaymentResponse.UserTokenId,
                        relatedTransactionId: initPaymentResponse.TransactionId,
                        billingAddress: new UserAddress
                        {
                            Country = "US",
                            Email = "john.smith@email.com",
                        },
                        deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }).GetAwaiter().GetResult();

                    if (paymentResponseOne.Status == ResponseStatus.Success)
                    {
                        // We need chanllenge.
                        return View("Challenge");

                    }
                    // We have finished as frictionless, show results.
                    return View("PaymentResult", paymentResponseOne);
                }
                else
                {
                    // We need fingerprinting.
                    //return View("BrowserFingerPrint", initPaymentResponse);
                    //Initialize the SDK (see https://docs.safecharge.com/documentation/guides/server-sdks/net-sdk/)

                    var paymentResponseWithFingerPrinting = _safecharge.Payment(
                        request.Currency,
                        request.Amount,
                        new PaymentOption
                        {
                            Card = new Card
                            {
                                CardNumber = request.PaymentOption.Card.CardNumber,
                                CardHolderName = request.PaymentOption.Card.CardHolderName,
                                ExpirationMonth = request.PaymentOption.Card.ExpirationMonth,
                                ExpirationYear = request.PaymentOption.Card.ExpirationYear,
                                CVV = request.PaymentOption.Card.CVV,
                                ThreeD = new ThreeD
                                {
                                    MethodCompletionInd = "Y",
                                    Version = "2.1.0",
                                    NotificationURL = request.PaymentOption.Card.ThreeD.MethodNotificationUrl ?? "https://localhost:44356/api/threeD/notification",
                                    MerchantURL = request.PaymentOption.Card.ThreeD.MethodNotificationUrl,
                                    PlatformType = "02",
                                    V2AdditionalParams = new V2AdditionalParams
                                    {
                                        ChallengeWindowSize = "05"
                                    },
                                    BrowserDetails = new BrowserDetails
                                    {
                                        AcceptHeader = "text/html,application/xhtml+xml",
                                        Ip = "192.168.1.11",
                                        JavaEnabled = "TRUE",
                                        JavaScriptEnabled = "TRUE",
                                        Language = "EN",
                                        ColorDepth = "48",
                                        ScreenHeight = "400",
                                        ScreenWidth = "600",
                                        TimeZone = "0",
                                        UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47"
                                    }
                                }
                            }
                        },
                        clientUniqueId: UNIQUEID,
                        clientRequestId: REQUESTID,
                        userTokenId: initPaymentResponse.UserTokenId,
                        relatedTransactionId: initPaymentResponse.TransactionId, // as returned from initPayment
                        billingAddress: new UserAddress
                        {
                            Country = "US",
                            Email = "john.smith@email.com",
                        },
                        deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" }).GetAwaiter().GetResult();
                    if (paymentResponseWithFingerPrinting.Status == ResponseStatus.Success)
                    {
                        return View("Challenge", paymentResponseWithFingerPrinting);
                    }

                    return View("PaymentResult", paymentResponseWithFingerPrinting);
                }
            }
        }


        [HttpPost]
        public IActionResult Challengev2([FromBody] PaymentResponse request)
        {
            return View(request);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        public IActionResult Payment(PaymentRequest request)
        {
            var ex = new SafechargeRequestExecutor();
            var response = ex.Payment(request).GetAwaiter().GetResult();
            //var response = _safecharge.Payment(
            //    request.Currency,
            //    request.Amount,
            //    request.PaymentOption,
            //    clientRequestId: REQUESTID,
            //    userTokenId: "230811147",
            //    clientUniqueId: "12345",
            //    deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });

            //if (response.Result == null) return RedirectToAction("Error");
            if (response == null) return RedirectToAction("Error");

            return View("PaymentSuccess", response);
        }

        [HttpPost]
        public IActionResult MethodUrl(string threeDSMethodData)
        {
            //threeDSMethodData: eyJ0aHJlZURTU2VydmVyVHJhbnNJRCI6IjY1YjIwYjY4LTNiOTItNGM0YS05OTc5LTkyYmYxMmFlODhmOSJ9
            string base64Encoded = threeDSMethodData;
            string base64Decoded;
            byte[] data = Convert.FromBase64String(base64Encoded);
            base64Decoded = Encoding.ASCII.GetString(data);
            return new StatusCodeResult((int)HttpStatusCode.Accepted);
        }
        private static List<Product> GetProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Blouse", Price = "40", ImageUrl1 = "~/img/p1.jpg", ImageUrl2 = "~/img/p2.jpg" },
                new Product { Id = 2, Name = "Shoe", Price = "40", ImageUrl1 = "~/img/p3.jpg", ImageUrl2 = "~/img/p4.jpg" },
                new Product { Id = 3, Name = "Back bag", Price = "20", ImageUrl1 = "~/img/p5.jpg", ImageUrl2 = "~/img/p6.jpg" },
                new Product { Id = 4, Name = "Pant", Price = "30", ImageUrl1 = "~/img/p7.jpg", ImageUrl2 = "~/img/p8.jpg" }
            };

            return products;
        }

        public static string GetChecksumString(string currency, string amount, string timestamp)
        {
            //merchantSecretKey = Secret1234
            //merchantId = 2389668057520747493
            //merchantSiteId = 199116
            //amount = 10
            //currency = EUR
            //timestamp = 2020-01-01 13:12:11
            //Secret1234238966805752074749319911610EUR2020-01-01 13:12:11
            //1c9becc3578b75b845228a03ec3aa11f6d0e3e680a3f49392fe4d269c74ae020

            var str = string.Concat(MERCHANTID, SITEID, REQUESTID, amount, currency.ToUpper(), timestamp, KEY);
            var checksum = ChecksumProvider.GetChecksumSha256(str);

            return checksum;
        }

        public static string GetChecksumString(string currency, string amount)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var str = string.Concat(KEY, MERCHANTID, SITEID, amount, currency.ToUpper(), REQUESTID, timestamp);
            var checksum = ChecksumProvider.GetChecksumSha256(str);

            return checksum;
        }
    }
}
