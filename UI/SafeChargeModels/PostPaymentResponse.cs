using System;
using System.Text.Json.Serialization;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class PostPaymentResponse
    {
        public string Ppp_status { get; set; }
        public string CardCompany { get; set; }
        public string NameOnCard { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Currency { get; set; }
        public string Merchant_site_id { get; set; }
        public string Merchant_id { get; set; }
        public string MerchantLocale { get; set; }
        public string RequestVersion { get; set; }
        [JsonPropertyName("PPP_TransactionID")]
        public string PPP_TransactionID { get; set; }
        public string ProductId { get; set; }
        public string CustomData { get; set; }
        public string Payment_method { get; set; }
        public string ResponseTimeStamp { get; set; }
        public string Message { get; set; }
        [JsonPropertyName("Error")]
        public string Error { get; set; }
        public string UserPaymentOptionId { get; set; }
        [JsonPropertyName("Status")]
        public string Status { get; set; }
        [JsonPropertyName("ExErrCode")]
        public string ExErrCode { get; set; }
        [JsonPropertyName("ErrCode")]
        public string ErrCode { get; set; }
        [JsonPropertyName("AuthCode")]
        public string AuthCode { get; set; }
        [JsonPropertyName("ReasonCode")]
        public string ReasonCode { get; set; }
        [JsonPropertyName("Token")]
        public string Token { get; set; }
        public string TokenId { get; set; }
        public string Responsechecksum { get; set; }
        public string AdvanceResponseChecksum { get; set; }
        public string TotalAmount { get; set; }
        [JsonPropertyName("TransactionID")]
        public string TransactionID { get; set; }
        public string DynamicDescriptor { get; set; }
        public string UniqueCC { get; set; }
        public string OrderTransactionId { get; set; }
        public string Item_amount_1 { get; set; }
        public string Item_quantity_1 { get; set; }

    }
}

// = OK
//&  = Visa
//&  = 123
//&  = CA
//&  = test@test.com
//&  = USD
//&  = 205838
//&  = 5305553900704185318
//&  = en_US
//&  = 4.0.0
//&  = 264413118
//&  = Blouse
//&  = Applause17
//&  = cc_card
//&  = 2020-08-15.17:25:12
//&  = Success
//&  = Success
//&  = 54398118
//&  = APPROVED
//&  = 0
//&  = 0
//&  = 111116
//&  = 0
//&  = ZQBVAEcAcwA0AFcAcAB3AGcATABDAFYAOgAtACgAQgBHAGwAUAAmADEAewBvAFIAMwBFADsARwBGADUAXAAtAGoAMgB7AE0AcQA6AGUAMwBDAFwAMwA =
//& = 133885845
//&  = 872e82e68db288df4dae8131230055ac46b53ef71c1038fb5d2a00a87e9b6dfa
//&  = 4f399da782bcaecc375afd5b2230f5c282068373ca53aadcf8e806892974b8f3
// &  = 80.00
//&  = 1110000000007489660
//&  = test 
//&  = ffD9XNu1GNvYPPAiInTBi + dUqOA = 
//& = 1051782288
//&  = 40.00
//&  = 2 &=