using Microsoft.AspNetCore.Mvc;
using Safecharge;
using Safecharge.Model.Common;
using Safecharge.Model.PaymentOptionModels.Verify3d;
using Safecharge.Request;
using Safecharge.Utils;
using Safecharge.Utils.Enum;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApp.Helper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreeDController : ControllerBase
    {
        private readonly ISafecharge _safecharge;
        private const string KEY = "0Sh4F6R2jQo6h8GYv62tabIOdgR4jqr2s41OMuMsLlB9RVFwmDeRtORN9m0Opj0W";
        private const string MERCHANTID = "2153502749988995611";
        private const string SITEID = "213146";
        private const string UNIQUEID = "12345";
        private const string REQUESTID = "20200807235612";

        public ThreeDController(ISafecharge safecharge)
        {
            _safecharge = safecharge;
        }

        // POST api/<ThreeDController>
        [HttpPost("getSessionToken")]
        public IActionResult GetSessionToken()
        {
            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);
            var re = new SafechargeRequestExecutor();
            var sessionTokenReq = new GetSessionTokenRequest(merchantInfo: merchantInfo);
            sessionTokenReq.ClientRequestId = REQUESTID;
            var sessionTokenRes = re.GetSessionToken(sessionTokenReq).GetAwaiter().GetResult();
            if (sessionTokenRes == null) return BadRequest();

            var sessionTokenReqJson = JsonSerializer.Serialize(sessionTokenReq);
            Console.WriteLine(sessionTokenReqJson);
            var sessionTokenResponseJson = System.Text.Json.JsonSerializer.Serialize(sessionTokenRes);
            Console.WriteLine(sessionTokenResponseJson);
            return Ok(sessionTokenRes);
        }

        // POST api/<ThreeDController>
        [HttpPost("initpayment")]
        public IActionResult InitPayment(InitPaymentRequest request)
        {
            var initPaymentResponse = _safecharge.InitPayment(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: request.PaymentOption ?? null,
                userTokenId: request.UserTokenId,
                orderId: request.OrderId,
                clientUniqueId: UNIQUEID,
                clientRequestId: REQUESTID,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" })
                .GetAwaiter().GetResult();
            if (initPaymentResponse == null) return BadRequest();
            var initPaymentResponseJson = JsonSerializer.Serialize(initPaymentResponse);
            Console.WriteLine(initPaymentResponseJson);
            return Ok(initPaymentResponse);
        }

        // POST api/<ThreeDController>
        [HttpPost("PaymentNon3D")]
        public async Task<IActionResult> PaymentNon3D(PaymentRequest request)
        {
            var response = await _safecharge.Payment(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: request.PaymentOption,
                clientRequestId: REQUESTID,
                userTokenId: "230811147",
                clientUniqueId: "12345",
                relatedTransactionId: request.RelatedTransactionId,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });

            if (response == null) return BadRequest();

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("PaymentMpi")]
        public async Task<IActionResult> PaymentMpiOnly(PaymentRequest request)
        {
            var response = await _safecharge.Payment(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: request.PaymentOption,
                clientRequestId: REQUESTID,
                userTokenId: "230811147",
                clientUniqueId: "12345",
                relatedTransactionId: request.RelatedTransactionId,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });

            if (response == null) return BadRequest();

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("PaymentSTS3D1")]
        public async Task<IActionResult> PaymentSTS3D1(PaymentRequest request)
        {
            // We are going to check if setting v2AdditionalParam2 returns success.
            if (request.PaymentOption.Card.ThreeD?.V2AdditionalParams == null)
            {
                request.PaymentOption.Card.ThreeD.V2AdditionalParams =
                    new Safecharge.Model.PaymentOptionModels.ThreeDModels.V2AdditionalParams
                    {
                        RebillExpiry = new DateTime(2021, 5, 2).ToString("yyyyMMdd"),
                        RebillFrequency = "2"
                    };
            }


            var response = await _safecharge.Payment(
                request.Currency,
                request.Amount,
                paymentOption: request.PaymentOption,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                userTokenId: request.UserTokenId,
                transactionType: request.TransactionType,
                relatedTransactionId: request.RelatedTransactionId,
                billingAddress: request.BillingAddress,
                deviceDetails: new DeviceDetails { IpAddress = "127.0.0.1" });
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);

            if (response == null) return BadRequest();
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("PaymentSTS3D2")]
        public async Task<IActionResult> PaymentSTS3D2(PaymentRequest request)
        {
            var response = await _safecharge.Payment(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: request.PaymentOption,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                userTokenId: request.UserTokenId,
                relatedTransactionId: request.RelatedTransactionId,
                billingAddress: request.BillingAddress,
                deviceDetails: request.DeviceDetails);

            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus([FromBody] string sessionToken)
        {
            var merchantInfo = new MerchantInfo(
                KEY,
                MERCHANTID,
                SITEID,
                ApiConstants.IntegrationHost,
                HashAlgorithmType.SHA256);

            GetPaymentStatusRequest req = new GetPaymentStatusRequest(merchantInfo: merchantInfo, sessionToken: sessionToken);

            var ex = new SafechargeRequestExecutor();
            var response = ex.GetPaymentStatus(req).GetAwaiter().GetResult();
            //var response = await _safecharge.GetPaymentStatus(sessionToken);

            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(sessionToken);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("SettleTransaction")]
        public async Task<IActionResult> SettleTransaction(SettleTransactionRequest request)
        {
            var response = await _safecharge.SettleTransaction(
                currency: request.Currency,
                amount: request.Amount,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                authCode: request.AuthCode,
                relatedTransactionId: request.RelatedTransactionId,
                deviceDetails: request.DeviceDetails);

            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("VoidTransaction")]
        public async Task<IActionResult> VoidTransaction(VoidTransactionRequest request)
        {
            var response = await _safecharge.VoidTransaction(
                currency: request.Currency,
                amount: request.Amount,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                authCode: request.AuthCode,
                relatedTransactionId: request.RelatedTransactionId,
                deviceDetails: request.DeviceDetails);

            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("RefundTransaction")]
        public async Task<IActionResult> RefundTransaction(VoidTransactionRequest request)
        {
            var response = await _safecharge.RefundTransaction(
                currency: request.Currency,
                amount: request.Amount,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                authCode: request.AuthCode,
                relatedTransactionId: request.RelatedTransactionId,
                deviceDetails: request.DeviceDetails);

            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }


        // POST api/<ThreeDController>
        [HttpPost("Authorize3D")]
        public async Task<IActionResult> Authorize3D(PaymentRequest request)
        {
            var response = await _safecharge.Authorize3d(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: request.PaymentOption,
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                userTokenId: request.UserTokenId,
                relatedTransactionId: request.RelatedTransactionId,
                billingAddress: request.BillingAddress,
                deviceDetails: request.DeviceDetails);
            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }

        // POST api/<ThreeDController>
        [HttpPost("Verify3D")]
        public async Task<IActionResult> Verify3D(PaymentRequest request)
        {
            var response = await _safecharge.Verify3d(
                currency: request.Currency,
                amount: request.Amount,
                paymentOption: new Safecharge.Model.PaymentOptionModels.Verify3d.Verify3dPaymentOption
                {
                    Card = new Verify3dCard
                    {
                        CardNumber = request.PaymentOption.Card.CardNumber,
                        CardHolderName = request.PaymentOption.Card.CardHolderName,
                        CcTempToken = request.PaymentOption.Card.CcTempToken,
                        CVV = request.PaymentOption.Card.CVV,
                        ExpirationMonth = request.PaymentOption.Card.ExpirationMonth,
                        ExpirationYear = request.PaymentOption.Card.ExpirationYear,
                    },

                },
                clientUniqueId: request.ClientUniqueId,
                clientRequestId: request.ClientRequestId,
                userTokenId: request.UserTokenId,
                relatedTransactionId: request.RelatedTransactionId,
                billingAddress: request.BillingAddress,
                deviceDetails: request.DeviceDetails);
            if (response == null) return BadRequest();
            var requestJson = JsonSerializer.Serialize(request);
            var responseJson = JsonSerializer.Serialize(response);
            Console.WriteLine(requestJson);
            Console.WriteLine(responseJson);

            return Ok(response);
        }


        // POST api/<ThreeDController>
        [HttpPost("NotificationUrl")]
        public IActionResult AcsUrl([FromBody] string threeDSMethodData)
        {
            string base64Encoded = threeDSMethodData;
            string base64Decoded;
            byte[] data = Convert.FromBase64String(base64Encoded);
            base64Decoded = Encoding.ASCII.GetString(data);
            return Accepted(base64Decoded);
        }

        [Route("notification")]
        [HttpPost()]
        public IActionResult Notification([FromBody] string threeDSMethodData)
        {
            if (string.IsNullOrEmpty(threeDSMethodData)) return BadRequest();
            string base64Encoded = threeDSMethodData;
            string base64Decoded;
            byte[] data = Convert.FromBase64String(base64Encoded);
            base64Decoded = Encoding.ASCII.GetString(data);
            return Ok(base64Decoded);
        }

        [Route("MethodNotificationUrl")]
        [HttpPost()]
        public IActionResult MethodNotificationUrl([FromBody] string threeDSMethodData)
        {
            if (string.IsNullOrEmpty(threeDSMethodData)) return BadRequest();
            string base64Encoded = threeDSMethodData;
            string base64Decoded;
            byte[] data = Convert.FromBase64String(base64Encoded);
            base64Decoded = Encoding.ASCII.GetString(data);
            return Ok(base64Decoded);
        }

        public string GetChecksumString(string currency, string amount, string timestamp)
        {
            var str = string.Concat(MERCHANTID, SITEID, amount, currency.ToUpper(), timestamp, KEY);
            var checksum = ChecksumProvider.GetChecksumSha256(str);

            return checksum;
        }
    }
}
