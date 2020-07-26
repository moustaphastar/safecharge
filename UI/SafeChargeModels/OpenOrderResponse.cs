using System;
using UI.Helpers;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class OpenOrderResponse
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(LongToStringJsonConverter))]
        public string SessionToken { get; set; }
        public string OrderId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        public string UserTokenId { get; set; }
        public string ClientUniqueId { get; set; }
        public string ClientRequestId { get; set; }
        public string InternalRequestId { get; set; }
        public string Status { get; set; }
        public string ErrCode { get; set; }
        public string Version { get; set; }
    }
}