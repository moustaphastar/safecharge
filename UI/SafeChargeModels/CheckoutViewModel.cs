using System;
using System.Collections.Generic;
using UI.Models;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class CheckoutViewModel
    {
        public string Secret { get; set; }
        public string MerchantId { get; set; }
        public string MerchantSiteId { get; set; }
        public string UserTokenId { get; set; }
        public string Version { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public List<CartItem> Items { get; set; }
        public string ItemListString { get; set; }
        public string TimeStamp { get; set; }
        public string Checksum { get; set; }
    }
}