using System;
using System.Text.Json.Serialization;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class PaymentStatusRequest
    {
        [JsonPropertyName("sessionToken")]
        public string SessionToken { get; set; }
    }
}