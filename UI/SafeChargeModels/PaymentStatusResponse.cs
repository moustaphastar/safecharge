using System;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class PaymentStatusResponse
    {
        public string GwExtendedErrorCode { get; set; }
        public string GwErrorCode { get; set; }
        public string GwErrorReason { get; set; }
        public string AuthCode { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public string UserTokenId { get; set; }
        public string TransactionId { get; set; }
        public PaymentOption PaymentOption { get; set; }
        public string SessionToken { get; set; }
        public string ClientUniqueId { get; set; }
        public string InternalRequestId { get; set; }
        public string Status { get; set; }
        public string ErrCode { get; set; }
        public string Reason { get; set; }
        public string MerchantSiteId { get; set; }
        public string Version { get; set; }
        public string ClientRequestId { get; set; }
    }

    [Serializable]
    public class PaymentOption
    {
        public string UserPaymentOptionId { get; set; }
        public Card Card { get; set; }
    }

    [Serializable]
    public class Card
    {
        public string UniqueCC { get; set; }
        public ThreeD ThreeD { get; set; }
    }

    [Serializable]
    public class ThreeD
    {
        public string IsLiabilityOnIssuer { get; set; }
    }
}