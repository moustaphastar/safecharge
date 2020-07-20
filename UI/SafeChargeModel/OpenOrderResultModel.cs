using System;
using System.ComponentModel.DataAnnotations;

namespace UI.SafeChargeModel
{
    [Serializable]
    public class OpenOrderResultModel
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(LongToStringJsonConverter))]
        public string SessionToken { get; set; }
        public string OrderId { get; set; }
        //[MaxLength(20)]
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        public string UserTokenId { get; set; }
        public string ClientUniqueId { get; set; }
        public string ClientRequestId { get; set; }
        public string InternalRequestId { get; set; }
        public string Status { get; set; }
        public string ErrCode { get; set; }
        public string Version { get; set; }


        //{
        //"sessionToken": "9610a8f6-44cf-4c4f-976a-005da69a2a3b",
        //"orderId": "39272",
        //"merchantId": "427583496191624621",
        //"merchantSiteId": "142033",
        //"userTokenId": "487106",
        //"clientUniqueId": "12345",
        //"clientRequestId": "1484759782197",
        //"internalRequestId": "866",
        //"status": "SUCCESS",
        //"errCode": "0",
        //"reason": "",
        //"version": "1.0"
        //}
    }
}