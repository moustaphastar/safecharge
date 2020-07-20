using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UI.SafeChargeModel
{
    [Serializable]
    public class OpenOrderModel
    {
        //[JsonPropertyName("Secret")]
        //public string Secret { get; set; }
        //[MaxLength(20)]
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        public string ClientUniqueId { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string TimeStamp { get; set; }
        public string Checksum { get; set; }
    }
}


//{
//    "merchantId": "427583496191624621",
//    "merchantSiteId": "142033",
//    "clientUniqueId": "12345",
//    "currency": "USD",
//    "amount": "10",
//    "deviceDetails": {
//        "ipAddress": "10.12.13.14"
//    },
//    "timeStamp": "20200118191751",
//    "checksum": "6b53666fc048a825be63cbb820da467b"
//}