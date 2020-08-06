using System;
using System.Text.Json.Serialization;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class PaymentRequest
    {
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        public string SessionToken { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string ClientRequestId { get; set; }
        public string UserTokenId { get; set; }
        public string ClientUniqueId { get; set; }
        public string OrderId { get; set; }
        public DeviceDetails DeviceDetails { get; set; }
        [JsonPropertyName("paymentOption")]
        public PaymentOptions PaymentOptions { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public string TimeStamp { get; set; }
        public string Checksum { get; set; }
    }

    [Serializable]
    public class PaymentOptions
    {
        [JsonPropertyName("card")]
        public Cards Card { get; set; }
    }

    [Serializable]
    public class Cards
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpirationMonth { get; set; }

        public string ExpirationYear { get; set; }
        public string CVV { get; set; }
    }

    [Serializable]
    public class DeviceDetails
    {
        public string IpAddress { get; set; }
    }

    [Serializable]
    public class BillingAddress
    {
        public string Country { get; set; }
        public string Email { get; set; }
    }
}
