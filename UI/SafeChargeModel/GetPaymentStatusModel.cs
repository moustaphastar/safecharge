using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace UI.SafeChargeModel
{
    [Serializable]
    public class GetPaymentStatusModel
    {
        [JsonPropertyName("sessionToken")]
        public string SessionToken { get; set; }
    }
}