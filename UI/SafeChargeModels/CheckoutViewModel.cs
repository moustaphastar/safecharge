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
        public string UserTokenId { get; set; } = "e5a4e1f7-28e5";
        public string Version { get; set; } = "4.0.0";
        public string Currency { get; set; }
        public string Amount { get; set; }
        public List<CartItem> Items { get; set; }
        public string ItemListString { get; set; }
        public string TotalTax { get; set; }
        public string Discount { get; set; }
        public string Shipping { get; set; }
        public string Handling { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; } = "CA";
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Locale { get; set; } = "en_US";
        public string Email { get; set; } = "test@test.com";
        public string PaymentMethod { get; set; } = "cc_card";
        public string TimeStamp { get; set; }
        public string Checksum { get; set; }
    }
}
