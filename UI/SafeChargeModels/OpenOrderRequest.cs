using System;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class OpenOrderRequest
    {
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        //public string ClientRequestId { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string TimeStamp { get; set; }
        public string Checksum { get; set; }
    }
}