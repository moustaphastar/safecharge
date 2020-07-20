using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.SafeChargeModel
{
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


    //"paymentOption": {
    //    "userPaymentOptionId": "14958143",
    //    "card": {
    //        "uniqueCC": "8/AOqY3oRC28rNNtUbtVPXjEtX0=",
    //        "threeD": {
    //            "isLiabilityOnIssuer": "1"
    //        }
    //    }
    //},